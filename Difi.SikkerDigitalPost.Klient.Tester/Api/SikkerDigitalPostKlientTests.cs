﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiClientShared;
using Difi.Felles.Utility.Exceptions;
using Difi.SikkerDigitalPost.Klient.Api;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Aktører;
using Difi.SikkerDigitalPost.Klient.Domene.Entiteter.Kvitteringer.Transport;
using Difi.SikkerDigitalPost.Klient.Domene.Exceptions;
using Difi.SikkerDigitalPost.Klient.Internal;
using Difi.SikkerDigitalPost.Klient.Tester.Fakes;
using Difi.SikkerDigitalPost.Klient.Tester.testdata.meldinger;
using Difi.SikkerDigitalPost.Klient.Tester.Utilities;
using Difi.SikkerDigitalPost.Klient.XmlValidering;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Difi.SikkerDigitalPost.Klient.Tester.Api
{
    [TestClass()]
    public class SikkerDigitalPostKlientTests
    {
        [TestClass]
        public class ConstructorMethod : SikkerDigitalPostKlientTests
        {
            [TestMethod]
            public void InitializesFields()
            {
                //Arrange
                var databehandler = new Databehandler(new Organisasjonsnummer("999999999"), DomeneUtility.GetAvsenderSertifikat());
                var klientkonfigurasjon = new Klientkonfigurasjon(Miljø.FunksjoneltTestmiljø);

                //Act
                var sikkerDigitalPostKlient = new  SikkerDigitalPostKlient(databehandler, klientkonfigurasjon);

                //Assert
                Assert.AreEqual(klientkonfigurasjon, sikkerDigitalPostKlient.Klientkonfigurasjon);
                Assert.AreEqual(databehandler, sikkerDigitalPostKlient.Databehandler);
                Assert.IsInstanceOfType(sikkerDigitalPostKlient.RequestHelper, typeof(RequestHelper));
            } 
        }

        [TestClass]
        public class SendMethod : SikkerDigitalPostKlientTests
        {
            [TestMethod]
            public async Task SuccessfullyReturnsTransportErrorReceipt()
            {
                //Arrange
                var sikkerDigitalPostKlient = DomeneUtility.GetSikkerDigitalPostKlientQaOffentlig();
                var fakeHttpClientHandlerResponse = new FakeHttpClientHandlerResponse(Resources.Xml.XmlResource.Response.GetTransportError().OuterXml, HttpStatusCode.BadRequest);
                sikkerDigitalPostKlient.RequestHelper.HttpClient = new HttpClient(fakeHttpClientHandlerResponse);

                //Act
                var message = DomeneUtility.GetDigitalForsendelseEnkel();
                var receipt = await sikkerDigitalPostKlient.SendAsync(message);

                //Assert
                Assert.IsInstanceOfType(receipt, typeof(TransportFeiletKvittering));
            }

            [ExpectedException(typeof(SdpSecurityException))]
            [TestMethod]
            public async Task ThrowsExceptionOnResponseNotMatchingRequest()
            {
                //Arrange
                var sikkerDigitalPostKlient = DomeneUtility.GetSikkerDigitalPostKlientQaOffentlig();
                var fakeHttpClientHandlerResponse = new FakeHttpClientHandlerResponse(Resources.Xml.XmlResource.Response.GetTransportOk().OuterXml, HttpStatusCode.OK);
                sikkerDigitalPostKlient.RequestHelper.HttpClient = new HttpClient(fakeHttpClientHandlerResponse);

                //Act
                var message = DomeneUtility.GetDigitalForsendelseEnkel();
                var receipt = await sikkerDigitalPostKlient.SendAsync(message);

                //Assert
                Assert.IsInstanceOfType(receipt, typeof(TransportFeiletKvittering));
            }
        }
    }
}