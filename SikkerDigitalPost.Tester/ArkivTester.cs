﻿/** 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *         http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SikkerDigitalPost.Domene.Entiteter.Aktører;
using SikkerDigitalPost.Klient.AsicE;

namespace SikkerDigitalPost.Tester
{
    [TestClass]
    public class ArkivTester : TestBase
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Initialiser();
            //Overkjør arkiv i Base for å bruke et sertifikat vi har privatekey til.
            Mottaker.Sertifikat = Mottakersertifikat();
        }

        [TestMethod]
        public void LeggFilerTilDokumentpakkeAntallStemmer()
        {
            Assert.AreEqual(Vedleggsstier.Length, Dokumentpakke.Vedlegg.Count);
            Assert.IsNotNull(Dokumentpakke.Hoveddokument);
        }

        [TestMethod]
        public void LagArkivOgVerifiserDokumentInnhold()
        {
            var dekryptertArkivBytes = AsicEArkiv.Dekrypter(Arkiv.Bytes);
            var arkivstrøm = new MemoryStream(dekryptertArkivBytes);

            //Åpne zip og generer sjekksum for å verifisere innhold
            using (var zip = new ZipArchive(arkivstrøm, ZipArchiveMode.Read))
            {
                //Alle vedlegg
                foreach (var filsti in Vedleggsstier)
                {
                    byte[] sjekksum1;
                    byte[] sjekksum2;

                    GenererSjekksum(zip, filsti, Path.GetFileName(filsti), out sjekksum1, out sjekksum2);
                    Assert.AreEqual(sjekksum1.ToString(), sjekksum2.ToString());
                }

                //Signaturfil
                {
                    byte[] sjekksum1;
                    byte[] sjekksum2;

                    GenererSjekksum(zip, Arkiv.Signatur.Bytes, Arkiv.Signatur.Filnavn, out sjekksum1, out sjekksum2);
                    Assert.AreEqual(sjekksum1.ToString(), sjekksum2.ToString());
                }

                //Manifest
                {
                    byte[] sjekksum1;
                    byte[] sjekksum2;

                    GenererSjekksum(zip, Arkiv.Manifest.Bytes, Path.GetFileName(Arkiv.Manifest.Filnavn), out sjekksum1, out sjekksum2);
                    Assert.AreEqual(sjekksum1.ToString(), sjekksum2.ToString());
                }
            }
        }

        [TestMethod]
        public void LagKryptertArkivVerifiserInnholdValiderer()
        {
            var arkiv = new AsicEArkiv(Forsendelse, GuidHandler, Databehandler.Sertifikat);
            var originalData = arkiv.Bytes;

            var krypterteData = arkiv.Bytes;
            var dekrypterteData = AsicEArkiv.Dekrypter(krypterteData); 

            Assert.AreEqual(originalData.ToString(), dekrypterteData.ToString());
        }
        
        private void GenererSjekksum(ZipArchive zip, string filstiPåDisk, string entryNavnIArkiv, out byte[] hash1, out byte[] hash2)
        {
            GenererSjekksum(zip, File.ReadAllBytes(filstiPåDisk), entryNavnIArkiv, out hash1, out hash2);
        }

        private void GenererSjekksum(ZipArchive zip, byte[] fil, string entryNavnIArkiv, out byte[] hash1, out byte[] hash2)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(fil))
                {
                    hash1 = md5.ComputeHash(stream);
                }

                using (var stream = zip.GetEntry(entryNavnIArkiv).Open())
                {
                    hash2 = md5.ComputeHash(stream);
                }
            }
        }

        private static X509Certificate2 Mottakersertifikat()
        {
            var storeMy = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeMy.Open(OpenFlags.ReadOnly);
            return storeMy.Certificates[0];
        }

    }
}
