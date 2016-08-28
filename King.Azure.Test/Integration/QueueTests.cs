namespace King.Service.Integration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using NUnit.Framework;

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
        public async Task ConstructorAccount()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var account = CloudStorageAccount.Parse(ConnectionString);
            var storage = new StorageQueue(name, account, TimeSpan.FromSeconds(34));
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }

        [Test]
        public async Task RoundTrip()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
        }

        [Test]
        public async Task RoundTripMsgAsObj()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send((object)msg);
            var returned = await storage.Get();

            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
        }
        
        [Test]
        public async Task RoundTripObject()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            var expected = Guid.NewGuid();
            await storage.Send(expected);

            var returned = await storage.Get();

            var guid = JsonConvert.DeserializeObject<Guid>(returned.AsString);

            Assert.AreEqual(expected, guid);
        }
        
        [Test]
        public async Task ApproixmateMessageCount()
        {
            var random = new Random();
            var count = random.Next(1, 1000);
            var storage = new StorageQueue(QueueName, ConnectionString);
            for (var i = 0; i < count; i++)
            {
                await storage.Send(Guid.NewGuid());
            }

            var result = await storage.ApproixmateMessageCount();
            Assert.AreEqual(count, result);
        }

        [Test]
        public async Task ApproixmateMessageCountNone()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);
            var result = await storage.ApproixmateMessageCount();
            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task Delete()
        {
            var storage = new StorageQueue(QueueName, ConnectionString);

            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);
            var returned = await storage.Get();
            await storage.Delete(returned);
        }

        [Test]
        public async Task RoundTripMany()
        {
            var random = new Random();
            var count = random.Next(1, 25);

            var storage = new StorageQueue(QueueName, ConnectionString);

            for (var i = 0; i < count; i++)
            {
                var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
                await storage.Send(msg);
            }

            var returned = await storage.GetMany(count);

            Assert.AreEqual(count, returned.Count());
        }

        [Test]
        public async Task GetManyNegative()
        {
            var random = new Random();
            var count = random.Next(1, 25);

            var storage = new StorageQueue(QueueName, ConnectionString);

            for (var i = 0; i < count; i++)
            {
                var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
                await storage.Send(msg);
            }

            var returned = await storage.GetMany(-1);

            Assert.AreEqual(1, returned.Count());
        }
    }
}