namespace Azure.Data.Wrappers.Test.Integration
{
    using Azure.Data.Wrappers;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    [Category("Integration")]
    public class QueueTests
    {
        private string GetQueueName()
        {
            return 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
        }

        private async Task<StorageQueue> QueueSetup()
        {
            var storage = new StorageQueue(GetQueueName(), TestHelpers.DevConnectionString);
            await storage.CreateIfNotExists();
            return storage;
        }

        [Test]
        public async Task CreateIfNotExists()
        {
            var name = GetQueueName();
            var storage = new StorageQueue(name, TestHelpers.DevConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
            await storage.Delete();
        }

        [Test]
        public async Task ConstructorAccount()
        {
            var name = GetQueueName();
            var account = CloudStorageAccount.Parse(TestHelpers.DevConnectionString);
            var storage = new StorageQueue(name, account, TimeSpan.FromSeconds(34));
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
            await storage.Delete();
        }

        [Test]
        public async Task RoundTrip()
        {
            StorageQueue storage = await QueueSetup();
            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
            await storage.Delete();
        }

        

        [Test]
        public async Task RoundTripMsgAsObj()
        {
            StorageQueue storage = await QueueSetup();

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send((object)msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
            await storage.Delete();
        }
        
        [Test]
        public async Task RoundTripObject()
        {
            StorageQueue storage = await QueueSetup();
            var expected = Guid.NewGuid();
            await storage.Send(expected);

            var returned = await storage.Get();

            var guid = JsonConvert.DeserializeObject<Guid>(returned.AsString);

            Assert.AreEqual(expected, guid);
            await storage.Delete();
        }
        
        [Test]
        public async Task ApproixmateMessageCount()
        {
            var random = new Random();
            var count = random.Next(1, 1000);
            StorageQueue storage = await QueueSetup();
            for (var i = 0; i < count; i++)
            {
                await storage.Send(Guid.NewGuid());
            }

            var result = await storage.ApproixmateMessageCount();
            Assert.AreEqual(count, result);
            await storage.Delete();
        }

        [Test]
        public async Task ApproixmateMessageCountNone()
        {
            StorageQueue storage = await QueueSetup();
            var result = await storage.ApproixmateMessageCount();
            Assert.AreEqual(0, result);
            await storage.Delete();
        }

        [Test]
        public async Task Delete()
        {
            StorageQueue storage = await QueueSetup();

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);
            var returned = await storage.Get();
            await storage.Delete(returned);
            await storage.Delete();
        }

        [Test]
        public async Task RoundTripMany()
        {
            var random = new Random();
            var count = random.Next(1, 25);

            StorageQueue storage = await QueueSetup();

            for (var i = 0; i < count; i++)
            {
                var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
                await storage.Send(msg);
            }

            var returned = await storage.GetMany(count);

            Assert.AreEqual(count, returned.Count());
            await storage.Delete();
        }

        [Test]
        public async Task GetManyNegative()
        {
            var random = new Random();
            var count = random.Next(1, 25);

            StorageQueue storage = await QueueSetup();

            for (var i = 0; i < count; i++)
            {
                var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
                await storage.Send(msg);
            }

            var returned = await storage.GetMany(-1);

            Assert.AreEqual(1, returned.Count());
            await storage.Delete();
        }
    }
}