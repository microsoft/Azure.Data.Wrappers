namespace King.Azure.Unit.Test.Data
{
    using System;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections.Generic;

    [TestFixture]
    public class StorageQueueShardsTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            var sqs = new StorageQueueShards("test", ConnectionString, 2);
            Assert.AreEqual(2, sqs.Queues.Count());
        }

        [Test]
        public void ConstructorConnectionNull()
        {
            Assert.That(() => new StorageQueueShards("test", null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorNameNull()
        {
            Assert.That(() => new StorageQueueShards(null, ConnectionString), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorQueuesNull()
        {
            Assert.That(() => new StorageQueueShards(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructorQueuesEmpty()
        {
            Assert.That(() => new StorageQueueShards(new List<IStorageQueue>()), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorShardDefault()
        {
            var sqs = new StorageQueueShards("test", ConnectionString);
            Assert.AreEqual(2, sqs.Queues.Count());
        }

        [Test]
        public void IsIQueueShardSender()
        {
            Assert.IsNotNull(new StorageQueueShards("test", ConnectionString) as IQueueShardSender<IStorageAccount>);
        }

        [Test]
        public void Queues()
        {
            var random = new Random();
            var i = random.Next(1, byte.MaxValue);
            var sqs = new StorageQueueShards("test", ConnectionString, 2);
            Assert.IsNotNull(sqs.Queues);
            Assert.AreEqual(i, sqs.Queues.Count());
        }
    }
}