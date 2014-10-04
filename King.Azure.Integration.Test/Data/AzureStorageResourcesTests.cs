namespace King.Azure.Integration.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class AzureStorageResourcesTests
    {
        private readonly string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public async Task Tables()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var tables = resources.Tables();

            Assert.IsTrue(tables.Contains(name));
        }

        [Test]
        public async Task Queues()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var queues = resources.Queues();

            Assert.IsTrue(queues.Contains(name));
        }

        [Test]
        public async Task Containers()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var containers = resources.Containers();

            Assert.IsTrue(containers.Contains(name));
        }
    }
}