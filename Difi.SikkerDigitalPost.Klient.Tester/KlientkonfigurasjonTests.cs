﻿using System;
using System.Linq;
using Difi.SikkerDigitalPost.Klient.XmlValidering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Difi.SikkerDigitalPost.Klient.Tester
{
    [TestClass]
    public class KlientkonfigurasjonTests
    {
        [TestClass]
        public class KonstruktørMethod : KlientkonfigurasjonTests
        {
            [TestMethod]
            public void InitializesProperties()
            {
                //Arrange
                var environment = Miljø.FunksjoneltTestmiljø;
                const string organizationNumberPosten = "984661185";
                object proxyhost = null;
                const string proxyScheme = "https";
                var timeoutInMilliseconds = (int) TimeSpan.FromSeconds(30).TotalMilliseconds;
                const int proxyPort = 0;

                //Act
                var clientConfiguration = new Klientkonfigurasjon(environment);

                //Assert
                Assert.AreEqual(environment, clientConfiguration.Miljø);
                Assert.AreEqual(organizationNumberPosten, clientConfiguration.MeldingsformidlerOrganisasjon.Verdi);
                Assert.AreEqual(proxyhost, clientConfiguration.ProxyHost);
                Assert.AreEqual(proxyScheme, clientConfiguration.ProxyScheme);
                Assert.AreEqual(timeoutInMilliseconds, clientConfiguration.TimeoutIMillisekunder);
                Assert.AreEqual(proxyPort, clientConfiguration.ProxyPort);
                Assert.AreEqual(clientConfiguration.LoggForespørselOgRespons, false);
            }
        }

        [TestClass]
        public class EnableDocumentBundleDiskDumpMethod : KlientkonfigurasjonTests
        {
            [TestMethod]
            public void AddsDocumentBundleToDiskProcessor()
            {
                //Arrange
                var clientConfiguration = new Klientkonfigurasjon(Miljø.FunksjoneltTestmiljø);

                //Act
                clientConfiguration.AktiverLagringAvDokumentpakkeTilDisk(@"\\vmware-host\Shared Folders\Downloads");

                //Assert
                Assert.IsTrue(clientConfiguration.Dokumentpakkeprosessorer.Any(p => p.GetType() == typeof (LagreDokumentpakkeTilDiskProsessor)));
            }
        }
    }
}