namespace King.Service.Integration
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class QueueTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";
        private const string QueueName = "testing";

        [SetUp]
        public void SetUp()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            storage.CreateIfNotExists().Wait();
        }

        [TearDown]
        public void TearDown()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            storage.Delete().Wait();
        }

        [Test]
        public async Task CreateIfNotExists()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }

        [Test]
        public async Task RoundTrip()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            await storage.CreateIfNotExists();
            
            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Save(msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
        }

        [Test]
        public async Task Delete()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            await storage.CreateIfNotExists();

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Save(msg);
            var returned = await storage.Get();
            await storage.Delete(returned);
        }
    }
}