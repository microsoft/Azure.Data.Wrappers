namespace King.Service.Integration
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    [TestFixture]
    public class ContainerTests
    {
        private readonly string ConnectionString = "UseDevelopmentStorage=true;";
        private const string ContainerName = "testing";

        #region Helper
        private class Helper
        {
            public Guid Id
            {
                get;
                set;
            }
        }
        #endregion

        [SetUp]
        public void SetUp()
        {
            var storage = new Container(ContainerName, ConnectionString);
            storage.CreateIfNotExists().Wait();
        }

        [TearDown]
        public void TearDown()
        {
            var storage = new Container(ContainerName, ConnectionString);
            storage.Delete().Wait();
        }

        [Test]
        public async Task ConstructorAccount()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var account = CloudStorageAccount.Parse(ConnectionString);
            var storage = new Container(name, account);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }

        [Test]
        public async Task CreateIfNotExists()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);

            var blobClient = storage.Account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(name);
            var permissions = await container.GetPermissionsAsync();
            Assert.AreEqual(BlobContainerPublicAccessType.Off, permissions.PublicAccess);
        }

        [Test]
        public async Task CreateIfNotExistsPublic()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString, true);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);

            var blobClient = storage.Account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(name);
            var permissions = await container.GetPermissionsAsync();
            Assert.AreEqual(BlobContainerPublicAccessType.Blob, permissions.PublicAccess);
        }

        [Test]
        public async Task Exists()
        {
            var helper = new Helper()
            {
                Id = Guid.NewGuid(),
            };

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            var exists = await storage.Exists(blobName);

            Assert.IsFalse(exists);

            await storage.Save(blobName, helper);
            exists = await storage.Exists(blobName);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task RoundTrip()
        {
            var helper = new Helper()
            {
                Id = Guid.NewGuid(),
            };

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, helper);
            var returned = await storage.Get<Helper>(blobName);

            Assert.IsNotNull(returned);
            Assert.AreEqual(helper.Id, returned.Id);
        }

        [Test]
        public async Task JsonContentType()
        {
            var helper = new Helper()
            {
                Id = Guid.NewGuid(),
            };

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, helper);
            var returned = await storage.Properties(blobName);

            Assert.IsNotNull(returned);
            Assert.AreEqual("application/json", returned.ContentType);
        }

        [Test]
        public async Task RoundTripBytes()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, bytes);
            var returned = await storage.Get(blobName);

            Assert.IsNotNull(returned);
            Assert.AreEqual(bytes.Length, returned.Length);
            Assert.AreEqual(bytes, returned);
        }

        [Test]
        public async Task RoundTripStream()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, bytes);
            using (var returned = await storage.Stream(blobName) as MemoryStream)
            {
                var stored = returned.ToArray();

                Assert.IsNotNull(stored);
                Assert.AreEqual(bytes.Length, stored.Length);
                Assert.AreEqual(bytes, stored);
            }
        }

        [Test]
        public async Task BytesDefaultContentType()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, bytes);
            var returned = await storage.Properties(blobName);

            Assert.IsNotNull(returned);
            Assert.AreEqual(bytes.Length, returned.Length);
            Assert.AreEqual("application/octet-stream", returned.ContentType);
        }

        [Test]
        public async Task BytesContentType()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, bytes, "application/pdf");
            var returned = await storage.Properties(blobName);

            Assert.IsNotNull(returned);
            Assert.AreEqual(bytes.Length, returned.Length);
            Assert.AreEqual("application/pdf", returned.ContentType);
        }

        [Test]
        [ExpectedException(typeof(StorageException))]
        public async Task Delete()
        {
            var helper = new Helper()
            {
                Id = Guid.NewGuid(),
            };

            var blobName = Guid.NewGuid().ToString();
            var storage = new Container(ContainerName, ConnectionString);

            await storage.Save(blobName, helper);
            await storage.Delete(blobName);
            await storage.Get<Helper>(blobName);
        }

        [Test]
        public async Task List()
        {
            var random = new Random();
            var bytes = new byte[16];
            random.NextBytes(bytes);
            var count = random.Next(1, 32);
            var storage = new Container(ContainerName, ConnectionString);
            for (var i = 0; i < count; i++)
            {
                var blobName = Guid.NewGuid().ToString();
                await storage.Save(blobName, bytes);
            }

            var blobs = storage.List();
            Assert.AreEqual(count, blobs.Count());
        }
    }
}