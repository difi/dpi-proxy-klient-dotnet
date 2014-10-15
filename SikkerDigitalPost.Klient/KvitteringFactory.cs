﻿using System;
using System.Xml;
using SikkerDigitalPost.Domene.Entiteter.Kvitteringer;
using SikkerDigitalPost.Klient.Envelope;

namespace SikkerDigitalPost.Klient
{
    internal class KvitteringFactory
    {

        public static Forretningskvittering GetForretningskvittering(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            if (IsLevertkvittering(xmlDocument))
            {
                return new Leveringskvittering(xmlDocument, NamespaceManager(xmlDocument));
            }
            if (IsVarslingFeiletkvittering(xmlDocument))
            {
                return new VarslingFeiletKvittering(xmlDocument, NamespaceManager(xmlDocument));
            }
            if (IsFeiletkvittering(xmlDocument))
            {
                return new Feilmelding(xmlDocument, NamespaceManager(xmlDocument));
            }
            if (IsÅpningskvittering(xmlDocument))
            {
                return new Åpningskvittering(xmlDocument, NamespaceManager(xmlDocument));
            }

            throw new Exception("Speccen har endra sæ, så du har fått ei TransportFeiletKvittering hær! Ikkebra. Nei.");
        }

        public static Transportkvittering GetTransportkvittering(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            if (IsTransportOkKvittering(xmlDocument))
            {
                return new TransportOkKvittering(xmlDocument, NamespaceManager(xmlDocument));
            }
            if (IsTransportFeiletKvittering(xmlDocument))
            {
                return new TransportFeiletKvittering(xmlDocument, NamespaceManager(xmlDocument));
            }

            throw new Exception("Du har fått ei transportkvittering såm vi ike hadde før. Lol.");
        }

        private static bool IsLevertkvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "ns9:kvittering");
        }

        private static bool IsVarslingFeiletkvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "ns9:varslingfeilet");
        }

        private static bool IsFeiletkvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "ns9:feil");
        }

        private static bool IsÅpningskvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "ns9:aapning");
        }

        private static bool IsTransportOkKvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "ns6:Receipt");
        }

        private static bool IsTransportFeiletKvittering(XmlDocument document)
        {
            return DocumentHasNode(document, "env:Fault");
        }

        private static bool DocumentHasNode(XmlDocument document, string node)
        {
            return DocumentNode(document, node) != null;
        }

        private static XmlNode DocumentNode(XmlDocument document, string node)
        {
            var rot = document.DocumentElement;
            string nodeString = String.Format("//{0}", node);
            var targetNode = rot.SelectSingleNode(nodeString, NamespaceManager(document));

            return targetNode;
        }

        private static XmlNamespaceManager NamespaceManager(XmlDocument document)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
            manager.AddNamespace("env", Navnerom.env);
            manager.AddNamespace("eb", Navnerom.eb);
            manager.AddNamespace("ns3", Navnerom.Ns3);
            manager.AddNamespace("ns5", Navnerom.Ns5);
            manager.AddNamespace("ns6", Navnerom.Ns6);
            manager.AddNamespace("ns9", Navnerom.Ns9);

            return manager;
        }
    }
}