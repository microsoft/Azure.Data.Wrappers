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
        public async Task TableNames()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var tables = resources.TableNames();

            Assert.IsTrue(tables.Contains(name));
        }

        [Test]
        public async Task QueueNames()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var queues = resources.QueueNames();

            Assert.IsTrue(queues.Contains(name));
        }

        [Test]
        public async Task ContainerNames()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var containers = resources.ContainerNames();

            Assert.IsTrue(containers.Contains(name));
        }
    }
}