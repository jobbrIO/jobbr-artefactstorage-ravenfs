﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Tests.Helpers;

namespace Jobbr.ArtefactStorage.RavenFS.Tests
{
    [TestClass]
    public class RavenFsArtefactStorageProviderTests : RavenTestBase
    {
        private DocumentStore _documentStore;
        private RavenFsConfiguration _ravenFsConfiguration;

        private void GivenRavenFs()
        {
            _documentStore = NewRemoteDocumentStore();
            _documentStore.Initialize();
        }

        private void GivenRavenFsConfiguration()
        {
            _ravenFsConfiguration = new RavenFsConfiguration
            {
                Url = _documentStore.Url,
                FileSystem = _documentStore.DefaultDatabase
            };
        }

        protected override void ModifyStore(EmbeddableDocumentStore documentStore)
        {
            base.ModifyStore(documentStore);

            documentStore.Configuration.Storage.Voron.AllowOn32Bits = true;
        }

        [TestMethod]
        public void GivenRavenFs_WhenQueryingForNonExistingContainer_ReturnsEmptyList()
        {
            GivenRavenFs();
            GivenRavenFsConfiguration();

            var storageProvider = new RavenFsArtefactStorageProvider(_ravenFsConfiguration);

            var artefacts = storageProvider.GetArtefacts("non-existing-container");

            Assert.AreEqual(0, artefacts.Count);
        }

        [TestMethod]
        public async Task GivenOneFileInContainer_WhenQueringAllFilesFromContainer_ReturnsThatFile()
        {
            GivenRavenFs();
            GivenRavenFsConfiguration();

            var storageProvider = new RavenFsArtefactStorageProvider(_ravenFsConfiguration);

            const string text = "lorem ipsum";
            storageProvider.Save("test-container", "file1.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text)));

            WaitForIndexing(_documentStore);

            var test = storageProvider.GetArtefacts("test-container");

            Assert.AreEqual(1, test.Count, "file1.txt must be in the list");

            var textLoaded = System.Text.Encoding.UTF8.GetString(test[0].Data.ReadInMemoryToEnd());

            Assert.AreEqual(text, textLoaded);
        }

        [TestMethod]
        public void GivenMultipleFilesInContainer_WhenQueringAllFilesFromContainer_ReturnsAllFiles()
        {
            GivenRavenFs();
            GivenRavenFsConfiguration();

            var storageProvider = new RavenFsArtefactStorageProvider(_ravenFsConfiguration);

            const string text1 = "lorem ipsum";
            const string text2 = "blub";
            storageProvider.Save("test-container", "file1.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text1)));
            storageProvider.Save("test-container", "file2.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text2)));

            WaitForIndexing(_documentStore, _ravenFsConfiguration.FileSystem);

            var test = storageProvider.GetArtefacts("test-container");

            Assert.AreEqual(2, test.Count, "both files must be in the list");

            var text1Loaded = System.Text.Encoding.UTF8.GetString(test[0].Data.ReadInMemoryToEnd());
            var text2Loaded = System.Text.Encoding.UTF8.GetString(test[1].Data.ReadInMemoryToEnd());

            Assert.AreEqual(text1, text1Loaded);
            Assert.AreEqual(text2, text2Loaded);
        }

        [TestMethod]
        public void GivenMultipleFilesInContainer_WhenQueringAllFilesFromAnotherContainer_ReturnsNoFiles()
        {
            GivenRavenFs();
            GivenRavenFsConfiguration();

            var storageProvider = new RavenFsArtefactStorageProvider(_ravenFsConfiguration);

            const string text1 = "lorem ipsum";
            const string text2 = "blub";
            storageProvider.Save("test-container", "file1.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text1)));
            storageProvider.Save("test-container", "file2.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text2)));

            WaitForIndexing(_documentStore);

            var test = storageProvider.GetArtefacts("another-container");

            Assert.AreEqual(0, test.Count, "none of the items in test-container should be in the list");
        }
    }
}