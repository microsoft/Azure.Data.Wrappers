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
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            await storage.CreateIfNotExists();
            
            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Save(msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
        }

        [Test]
        public async Task Delete()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new StorageQueue(name, ConnectionString);
            await storage.CreateIfNotExists();

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Save(msg);
            var returned = await storage.Get();
            await storage.Delete(returned);
        }
    }
}