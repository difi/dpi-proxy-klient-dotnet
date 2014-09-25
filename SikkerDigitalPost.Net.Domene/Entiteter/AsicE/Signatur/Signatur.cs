﻿using System.IO;
using System.Security.Cryptography.X509Certificates;
using SikkerDigitalPost.Net.Domene.Entiteter.Interface;

namespace SikkerDigitalPost.Net.Domene.Entiteter.AsicE.Signatur
{
    public class Signatur : IAsiceVedlegg
    {
        public readonly X509Certificate2 Sertifikat;

        public Signatur(X509Certificate2 sertifikat)
        {
            Sertifikat = sertifikat;
        }

        public string Filnavn {
            get { return "META-INF/signatures.xml"; } 
        }

        public byte[] Bytes { get; set; }

        public string Innholdstype {
            get { return "application/xml"; }
        }
    }
}
