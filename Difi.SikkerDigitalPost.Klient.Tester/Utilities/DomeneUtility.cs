﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ApiClientShared;
using ApiClientShared.Enums;
using Difi.SikkerDigitalPost.Klient.Api;
using Difi.SikkerDigitalPost.Klient.AsicE;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Aktører;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.FysiskPost;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Kvitteringer.Forretning;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Post;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Varsel;
using Difi.SikkerDigitalPost.Klient.Domene.Enums;
using Difi.SikkerDigitalPost.Klient.Envelope;
using Difi.SikkerDigitalPost.Klient.Envelope.Forretningsmelding;
using Difi.SikkerDigitalPost.Klient.Tester.Properties;
using Difi.SikkerDigitalPost.Klient.Utilities;
using Difi.SikkerDigitalPost.Klient.XmlValidering;

namespace Difi.SikkerDigitalPost.Klient.Tester.Utilities
{
    /// <summary>
    /// Hjelpeklasse for instansiering av domeneobjekter. Klassen kan virke tilstandsløs, og vil for alle praktiske formål være det,
    /// da man vil få det samme tilbake hver gang / deterministisk. Likevel er det viktig å vite at filobjekter vil leses fra disk kun èn
    /// gang for økt ytelse.
    /// </summary>
    internal static class DomeneUtility
    {
        internal static readonly ResourceUtility ResourceUtility = new ResourceUtility("Difi.SikkerDigitalPost.Klient.Tester.testdata");

        private static readonly GuidUtility GuidUtility = new GuidUtility();

        private static Dokument _hoveddokument;

        private static IEnumerable<Dokument> _vedlegg;

        internal static Dokumentpakke GetDokumentpakkeUtenVedlegg()
        {
            var dokumentpakke = new Dokumentpakke(GetHoveddokumentEnkel());
            return dokumentpakke;
        }

        internal static Dokumentpakke GetDokumentpakkeMedFlereVedlegg(int antall = 3)
        {
            var dokumentpakke = new Dokumentpakke(GetHoveddokumentEnkel());
            dokumentpakke.LeggTilVedlegg(GetVedlegg(antall));
            return dokumentpakke;
        }

        internal static Dokument GetHoveddokumentEnkel()
        {
            if (_hoveddokument != null)
            {
                return _hoveddokument;
            }

            return _hoveddokument = new Dokument("Hoveddokument", ResourceUtility.ReadAllBytes(true, "hoveddokument", "Hoveddokument.pdf"), "application/pdf");
        }

        internal static string[] GetVedleggsFilerStier()
        {
            const string vedleggsMappe = "vedlegg";

            return ResourceUtility.GetFiles(vedleggsMappe).ToArray();
        }

        internal static IEnumerable<Dokument> GetVedlegg(int maksAntall = 5)
        {
            if (_vedlegg != null)
            {
                return _vedlegg;
            }

            var vedleggTxt0 = new Dokument("Vedlegg", ResourceUtility.ReadAllBytes(true, "vedlegg", "Vedlegg.txt"), "text/plain");
            var vedleggDocx = new Dokument("Vedleggsgris", ResourceUtility.ReadAllBytes(true, "vedlegg", "VedleggsGris.docx"), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            var vedleggPdf = new Dokument("Vedleggshjelm", ResourceUtility.ReadAllBytes(true, "vedlegg", "VedleggsHjelm.pdf"), "application/pdf");
            var vedleggTxt1 = new Dokument("Vedlegg", ResourceUtility.ReadAllBytes(true, "vedlegg", "Vedlegg.txt"), "text/plain");
            var vedleggTxt2 = new Dokument("Vedlegg", ResourceUtility.ReadAllBytes(true, "vedlegg", "Vedlegg.txt"), "text/plain");
            
            _vedlegg = new[] { vedleggTxt0, vedleggDocx, vedleggPdf, vedleggTxt1, vedleggTxt2 };

            return _vedlegg.Take(maksAntall);

        }

        internal static string GetMimeType(string fileName)
        {
            return "text/plain";
        }

        internal static Avsender GetAvsender()
        {
            var orgNrAvsender = new Organisasjonsnummer(Settings.Default.OrganisasjonsnummerAvsender);
            return new Avsender(orgNrAvsender) { Avsenderidentifikator = Settings.Default.Avsenderidentifikator };
        }

        internal static DigitalPostMottaker GetDigitalPostMottaker()
        {
            return new DigitalPostMottaker(Settings.Default.PersonnummerMottaker, Settings.Default.DigitalPostkasseAdresseMottaker, GetMottakerSertifikat(), Settings.Default.OrganisasjonsnummerPostkasse);
        }

        internal static DigitalPostMottaker GetDigitalPostMottakerMedTestSertifikat()
        {
            return new DigitalPostMottaker(Settings.Default.PersonnummerMottaker, Settings.Default.DigitalPostkasseAdresseMottaker, GetMottakerEnhetstesterSertifikat(), Settings.Default.OrganisasjonsnummerPostkasse);
        }

        internal static FysiskPostMottaker GetFysiskPostMottaker()
        {
            return new FysiskPostMottaker("Testbruker i Tester .NET", new NorskAdresse("0001", "Testekommunen"), GetMottakerSertifikat(), Settings.Default.OrganisasjonsnummerPostkasse);
        }

        internal static FysiskPostMottaker GetFysiskPostMottakerMedTestSertifikat()
        {
            return new FysiskPostMottaker("Testbruker i Tester .NET med testsertifikat", new NorskAdresse("0001", "Testekommunen"), GetMottakerEnhetstesterSertifikat(), Settings.Default.OrganisasjonsnummerPostkasse);
        }

        internal static FysiskPostReturmottaker GetFysiskPostReturMottaker()
        {

            return new FysiskPostReturmottaker("Testbruker i Tester .NET", new NorskAdresse("0001", "Testekommunen"));
        }

        internal static Databehandler GetDatabehandler()
        {
            return new Databehandler(GetAvsender().Organisasjonsnummer, GetAvsenderSertifikat());
        }

        internal static Databehandler GetDatabehandlerMedTestSertifikat()
        {
            return new Databehandler(GetAvsender().Organisasjonsnummer, GetAvsenderEnhetstesterSertifikat());
        }

        internal static DigitalPostInfo GetDigitalPostInfoMedVarsel()
        {
            return new DigitalPostInfo(GetDigitalPostMottaker(), "Ikke-sensitiv tittel")
            {
                EpostVarsel = new EpostVarsel("tull@ball.no", "Dette er et epostvarsel fra Enhentstester .NET", 0, 7),
                SmsVarsel = new SmsVarsel("45215454", "Dette er et smsvarsel fra Enhetstester .NET", 3, 14)
            };
        }

        internal static DigitalPostInfo GetDigitalPostInfoEnkel()
        {
            return new DigitalPostInfo(GetDigitalPostMottaker(), "Ikke-sensitiv tittel");
        }

        internal static DigitalPostInfo GetDigitalPostInfoEnkelMedTestSertifikat()
        {
            return new DigitalPostInfo(GetDigitalPostMottakerMedTestSertifikat(), "Ikke-sensitiv tittel");
        }

        internal static FysiskPostInfo GetFysiskPostInfoEnkel()
        {
            return new FysiskPostInfo(GetFysiskPostMottaker(), Posttype.A, Utskriftsfarge.Farge,
                Posthåndtering.DirekteRetur, GetFysiskPostReturMottaker());
        }

        internal static Forsendelse GetDigitalForsendelseEnkel()
        {
            return new Forsendelse(GetAvsender(), GetDigitalPostInfoEnkel(), GetDokumentpakkeUtenVedlegg(), Prioritet.Normal, mpcId: Guid.NewGuid().ToString());
        }

        internal static Forsendelse GetDigitalForsendelseEnkelMedTestSertifikat()
        {
            return new Forsendelse(GetAvsender(), GetDigitalPostInfoEnkelMedTestSertifikat(), GetDokumentpakkeUtenVedlegg(), Prioritet.Normal, mpcId: Guid.NewGuid().ToString());
        }

        internal static Forsendelse GetFysiskForsendelseEnkel()
        {
            return new Forsendelse(GetAvsender(), GetFysiskPostInfoEnkel(), GetDokumentpakkeUtenVedlegg(), Prioritet.Normal, mpcId: Guid.NewGuid().ToString());
        }

        internal static Forsendelse GetDigitalForsendelseVarselFlereDokumenterHøyereSikkerhet()
        {
            return new Forsendelse(GetAvsender(), GetDigitalPostInfoMedVarsel(), GetDokumentpakkeMedFlereVedlegg(), Prioritet.Normal, Guid.NewGuid().ToString());
        }

        internal static AsicEArkiv GetAsicEArkivEnkel()
        {

            return new AsicEArkiv(GetDigitalForsendelseEnkel(), GuidUtility, GetAvsenderSertifikat());
        }

        internal static AsicEArkiv GetAsicEArkivEnkelMedTestSertifikat()
        {

            return new AsicEArkiv(GetDigitalForsendelseEnkelMedTestSertifikat(), GuidUtility, GetAvsenderEnhetstesterSertifikat());
        }
        
        internal static AsicEArkiv GetAsicEArkiv(Forsendelse forsendelse)
        {
            return new AsicEArkiv(forsendelse, GuidUtility, GetAvsenderEnhetstesterSertifikat());
        }

        internal static ForretningsmeldingEnvelope GetForretningsmeldingEnvelopeMedTestSertifikat()
        {
            var envelopeSettings = new EnvelopeSettings(
                GetDigitalForsendelseEnkelMedTestSertifikat(),
                GetAsicEArkivEnkelMedTestSertifikat(),
                GetDatabehandlerMedTestSertifikat(),
                GuidUtility,
                new Klientkonfigurasjon(Miljø.FunksjoneltTestmiljø));
            return new ForretningsmeldingEnvelope(envelopeSettings);
        }

        internal static ForretningsmeldingEnvelope GetForretningsmeldingEnvelope()
        {
            var envelopeSettings = new EnvelopeSettings(
                GetDigitalForsendelseEnkel(),
                GetAsicEArkivEnkel(),
                GetDatabehandler(),
                GuidUtility,
                new Klientkonfigurasjon(Miljø.FunksjoneltTestmiljø));
            return new ForretningsmeldingEnvelope(envelopeSettings);
        }

        internal static SikkerDigitalPostKlient GetSikkerDigitalPostKlientQaOffentlig()
        {
            return new SikkerDigitalPostKlient(GetDatabehandler(), new Klientkonfigurasjon(Miljø.FunksjoneltTestmiljø));
        }

        internal static X509Certificate2 GetAvsenderEnhetstesterSertifikat()
        {
            return EvigTestSertifikatMedPrivatnøkkel();
        }

        internal static X509Certificate2 GetMottakerEnhetstesterSertifikat()
        {
            return EvigTestSertifikatUtenPrivatnøkkel();
        }

        private static X509Certificate2 EvigTestSertifikatUtenPrivatnøkkel()
        {
            return new X509Certificate2(ResourceUtility.ReadAllBytes(true, "sertifikater", "enhetstester", "difi-enhetstester.cer"), "", X509KeyStorageFlags.Exportable);
        }

        private static X509Certificate2 EvigTestSertifikatMedPrivatnøkkel()
        {
            return new X509Certificate2(ResourceUtility.ReadAllBytes(true, "sertifikater","enhetstester", "difi-enhetstester.p12"), "", X509KeyStorageFlags.Exportable);
        }

        internal static X509Certificate2 GetAvsenderSertifikat()
        {
            return CertificateUtility.SenderCertificate(Settings.Default.DatabehandlerSertifikatThumbprint, Language.Norwegian);
        }

        internal static X509Certificate2 GetMottakerSertifikat()
        {
            return new X509Certificate2(ResourceUtility.ReadAllBytes(true, "sertifikater", "test", "testmottakersertifikatFraOppslagstjenesten.pem"));
        }

        internal static Leveringskvittering GetLeveringskvittering()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";
            
            return new Leveringskvittering(konversasjonsId, bodyReferenceUri, digestValue);
        }

        internal static Mottakskvittering GetMottakskvittering()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";

            return new Mottakskvittering(konversasjonsId, bodyReferenceUri, digestValue);
        }

        public static Returpostkvittering GetReturpostkvittering()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";

            return new Returpostkvittering(konversasjonsId, bodyReferenceUri, digestValue);
        }

        public static VarslingFeiletKvittering GetVarslingFeiletKvittering()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";

            return new VarslingFeiletKvittering(konversasjonsId, bodyReferenceUri, digestValue);
        }

        public static Åpningskvittering GetÅpningskvittering()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";

            return new Åpningskvittering(konversasjonsId, bodyReferenceUri, digestValue);
        }

        public static Feilmelding GetFeilmelding()
        {
            var konversasjonsId = Guid.NewGuid();
            var bodyReferenceUri = "bodyReferenceUri";
            var digestValue = "digestValue";

            return new Feilmelding(konversasjonsId, bodyReferenceUri, digestValue);

        }
    }
}