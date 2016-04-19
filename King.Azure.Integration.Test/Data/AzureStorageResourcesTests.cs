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
        public async Task Tables()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var tables = resources.Tables();

            var exists = (from t in tables
                          where t.Name == name
                          select true).FirstOrDefault();

            Assert.IsTrue(exists);
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
        public async Task Queues()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var queues = resources.Queues();

            var exists = (from q in queues
                          where q.Name == name
                          select true).FirstOrDefault();

            Assert.IsTrue(exists);

            await storage.Delete();
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

        [Test]
        public async Task Containers()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            var resources = new AzureStorageResources(ConnectionString);
            var containers = resources.Containers();

            var exists = (from c in containers
                          where c.Name == name
                          select true).FirstOrDefault();

            Assert.IsTrue(exists);
        }
    }
}