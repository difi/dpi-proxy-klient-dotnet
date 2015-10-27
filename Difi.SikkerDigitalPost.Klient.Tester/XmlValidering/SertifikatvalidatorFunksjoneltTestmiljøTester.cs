﻿using System.Security.Cryptography.X509Certificates;
using ApiClientShared;
using Difi.SikkerDigitalPost.Klient.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Difi.SikkerDigitalPost.Klient.XmlValidering.Tests
{
    [TestClass]
    public class SertifikatvalidatorFunksjoneltTestmiljøTester
    {
        readonly ResourceUtility _resourceUtility = new ResourceUtility("Difi.SikkerDigitalPost.Klient.Tester.testdata.sertifikater");

        [TestClass]
        public class ErGyldigResponssertifikatMethod : SertifikatvalidatorFunksjoneltTestmiljøTester
        {
            [TestMethod]
            public void GodkjennerTestsertifikatReturnererKjedeSomOutparameter()
            {
                //Arrange
                var testSertifikat = new X509Certificate2(_resourceUtility.ReadAllBytes(true, "test", "testmottakerFraOppslagstjenesten.pem"));

                //Act
                SertifikatValidatorFunksjoneltTestmiljø sertifikatValidator = new SertifikatValidatorFunksjoneltTestmiljø(SertifikatUtility.TestSertifikater());
                X509ChainStatus[] kjedestatus;
                var result = sertifikatValidator.ErGyldigResponssertifikat(testSertifikat, out kjedestatus);

                //Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, kjedestatus.Length);
            }

            [TestMethod]
            public void GodkjennerTestsertifikat()
            {
                //Arrange
                var testSertifikat = new X509Certificate2(_resourceUtility.ReadAllBytes(true, "test", "testmottakerFraOppslagstjenesten.pem"));

                //Act
                SertifikatValidatorFunksjoneltTestmiljø sertifikatValidator = new SertifikatValidatorFunksjoneltTestmiljø(SertifikatUtility.TestSertifikater());
                var result = sertifikatValidator.ErGyldigResponssertifikat(testSertifikat);

                //Assert
                Assert.IsTrue(result);
            }
        }
    }
}