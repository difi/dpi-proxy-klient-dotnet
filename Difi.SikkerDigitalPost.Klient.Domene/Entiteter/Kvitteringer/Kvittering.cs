﻿using System;
using System.Xml;
using Difi.SikkerDigitalPost.Klient.Domene.Exceptions;

namespace Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Kvitteringer
{
    public abstract class Kvittering
    {
        private readonly XmlDocument _document;
        private readonly XmlNamespaceManager _namespaceManager;

        /// <summary>
        /// Tidspunktet da kvitteringen ble sendt.
        /// </summary>
        public DateTime SendtTidspunkt { get; set; }

        /// <summary>
        /// Unik identifikator for kvitteringen.
        /// </summary>
        public string MeldingsId { get; set; }

        /// <summary>
        /// Refereranse til en annen relatert melding. Refererer til den relaterte meldingens MessageId.
        /// </summary>
        public string ReferanseTilMeldingId { get; set; }

        /// <summary>
        /// Kvitteringen presentert som tekststreng.
        /// </summary>
        public string Rådata { get; set; }

        protected Kvittering() { }

        protected Kvittering(XmlDocument document, XmlNamespaceManager namespaceManager)
        {
            _document = document;
            _namespaceManager = namespaceManager;

            try
            {
                SendtTidspunkt = Convert.ToDateTime(DocumentNode("//ns6:Timestamp").InnerText);
                MeldingsId = DocumentNode("//ns6:MessageId").InnerText;

                var referanseTilMeldingId = DocumentNode("//ns6:RefToMessageId");
                if (referanseTilMeldingId != null)
                {
                    ReferanseTilMeldingId = referanseTilMeldingId.InnerText;
                }
                Rådata = document.OuterXml;
            }
            catch (Exception e)
            {
                throw new XmlParseException(
                    String.Format(
                        "Feil under bygging av {0} (av type Kvittering). Klarte ikke finne alle felter i xml."
                        , GetType()), e);
            }

        }

        protected XmlNode DocumentNode(string xPath)
        {
            try
            {
                var rot = _document.DocumentElement;
                var targetNode = rot.SelectSingleNode(xPath, _namespaceManager);

                return targetNode;
            }
            catch (Exception e)
            {
                throw new XmlParseException(
                    String.Format("Feil under henting av dokumentnode i {0} (av type Forretningskvittering). Klarte ikke finne alle felter i xml."
                    , GetType()), e);
            }
        }

        protected string ToString()
        {
            return string.Format("SendtTidspunkt: {0}, MeldingsId: {1}, ReferanseTilMeldingId: {2}", SendtTidspunkt, MeldingsId, ReferanseTilMeldingId);
        }
    }
}
