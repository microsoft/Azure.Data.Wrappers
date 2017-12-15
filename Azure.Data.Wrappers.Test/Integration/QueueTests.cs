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
    using System.Collections.Generic;

    [TestFixture]
    [Category("Integration")]
    public class QueueTests
    {
        private class TestCustomObject
        {
            public string FooString { get; set; }
            public int FooInt { get; set; }
        }
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
            var expected = new TestCustomObject { FooInt = 42, FooString = "The Answer" };
            await storage.SendAsync<TestCustomObject>(expected);

            var returned = await storage.Get();

            var deserialized = JsonConvert.DeserializeObject<TestCustomObject>(returned.AsString);

            Assert.AreEqual(expected.FooString, deserialized.FooString);
            Assert.AreEqual(expected.FooInt, deserialized.FooInt  );
            await storage.Delete();
        }

        [Test]
        public async Task GenericGet()
        {
            StorageQueue storage = await QueueSetup();
            var expected = new TestCustomObject { FooInt = 42, FooString = "The Answer" };
            await storage.SendAsync(expected);

            var returned = await storage.GetAsync<TestCustomObject>();

            Assert.AreEqual(expected.FooString, returned.FooString);
            Assert.AreEqual(expected.FooInt, returned.FooInt);
            await storage.Delete();
        }

        [Test]
        public async Task GenericGetMany()
        {
            StorageQueue storage = await QueueSetup();
            var expected = new TestCustomObject { FooInt = 42, FooString = "The Answer" };
            var expected2 = new TestCustomObject { FooInt = 43, FooString = "The Answer 2" };
            var expectedList = new List<TestCustomObject> { expected, expected2 };

            foreach (var item in expectedList)
            {
                await storage.SendAsync(item);
            }

            var returned = (await storage.GetManyAsync<TestCustomObject>()).ToList();

            Assert.AreEqual(expectedList.Count, returned.Count);
            Assert.IsTrue(returned.Any(m => m.FooInt == expected.FooInt && m.FooString == expected.FooString));
            Assert.IsTrue(returned.Any(m => m.FooInt == expected2.FooInt && m.FooString == expected2.FooString));
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
                await storage.SendAsync(Guid.NewGuid());
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

        [Test]
        public async Task Peek()
        {
            StorageQueue storage = await QueueSetup();
            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);

            var peeked = await storage.PeekAsync();
            Assert.AreEqual(msg.AsBytes, peeked.First().AsBytes);

            var returned = await storage.Get();
            Assert.AreEqual(msg.AsBytes, returned.AsBytes);
            await storage.Delete();
        }

        [Test]
        public async Task PeekGeneric()
        {
            StorageQueue storage = await QueueSetup();
            var expected = new TestCustomObject { FooInt = 42, FooString = "The Answer" };
            var expected2 = new TestCustomObject { FooInt = 43, FooString = "The Answer 2" };
            var expectedList = new List<TestCustomObject> { expected, expected2 };

            foreach (var item in expectedList)
            {
                await storage.SendAsync(item);
            }

            var peeked = (await storage.PeekAsync<TestCustomObject>(2)).ToList();
            Assert.AreEqual(expectedList.Count, peeked.Count);
            Assert.IsTrue(expectedList.Any(m => m.FooInt == expected.FooInt && m.FooString == expected.FooString));
            Assert.IsTrue(expectedList.Any(m => m.FooInt == expected2.FooInt && m.FooString == expected2.FooString));

            var returned = (await storage.GetManyAsync<TestCustomObject>()).ToList();
            Assert.AreEqual(expectedList.Count, returned.Count);
            Assert.IsTrue(returned.Any(m => m.FooInt == expected.FooInt && m.FooString == expected.FooString));
            Assert.IsTrue(returned.Any(m => m.FooInt == expected2.FooInt && m.FooString == expected2.FooString));
            await storage.Delete();
        }

        [Test]
        public async Task ClearAsync()
        {
            StorageQueue storage = await QueueSetup();
            var msg = new CloudQueueMessage(Guid.NewGuid().ToByteArray());
            await storage.Send(msg);

            await storage.ClearAsync();

            var returned = await storage.Get();
            Assert.IsNull(returned);
            await storage.Delete();
        }
    }
}