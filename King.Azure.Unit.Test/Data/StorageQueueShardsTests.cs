namespace King.Azure.Unit.Test.Data
{
    using System;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections.Generic;
    using NSubstitute;
    using System.Threading.Tasks;
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
            Assert.IsNotNull(new StorageQueueShards("test", ConnectionString) as IQueueShardSender<IStorageQueue>);
        }

        [Test]
        public void Queues()
        {
            var random = new Random();
            var i = (byte)random.Next(1, byte.MaxValue);
            var sqs = new StorageQueueShards("test", ConnectionString, i);
            Assert.IsNotNull(sqs.Queues);
            Assert.AreEqual(i, sqs.Queues.Count());
        }

        [Test]
        public async Task CreateIfNotExists()
        {
            var random = new Random();
            var i = random.Next(1, byte.MaxValue);
            var qs = new List<IStorageQueue>();
            for (var j = 0; j < i; j++)
            {
                var q = Substitute.For<IStorageQueue>();
                q.CreateIfNotExists().Returns(Task.FromResult(true));
                qs.Add(q);
            }
            var sqs = new StorageQueueShards(qs);

            var success = await sqs.CreateIfNotExists();
            Assert.IsTrue(success);

            foreach (var q in qs)
            {
                await q.Received().CreateIfNotExists();
            }
        }

        [Test]
        public async Task Delete()
        {
            var random = new Random();
            var i = random.Next(1, byte.MaxValue);
            var qs = new List<IStorageQueue>();
            for (var j = 0; j < i; j++)
            {
                var q = Substitute.For<IStorageQueue>();
                q.Delete().Returns(Task.FromResult(true));
                qs.Add(q);
            }
            var sqs = new StorageQueueShards(qs);

            await sqs.Delete();

            foreach (var q in qs)
            {
                await q.Received().Delete();
            }
        }

        [Test]
        public async Task Save()
        {
            var random = new Random();
            var i = (byte)random.Next(1, byte.MaxValue);

            var msg = new object();
            var q = Substitute.For<IStorageQueue>();
            q.Save(msg).Returns(Task.CompletedTask);
            var qs = Substitute.For<IReadOnlyCollection<IStorageQueue>>();
            qs.ElementAt(i).Returns(q);
            qs.Count().Returns(i);

            var sqs = new StorageQueueShards(qs);

            await sqs.Save(msg, i);

            qs.Received().Count();
            qs.Received().ElementAt(i);
            await q.Received().Save(msg);
        }

        [Test]
        public async Task SaveShardZero()
        {
            var random = new Random();
            var i = (byte)random.Next(1, byte.MaxValue);
            var msg = new object();
            var q = Substitute.For<IStorageQueue>();
            q.Save(msg).Returns(Task.CompletedTask);
            var qs = Substitute.For<IReadOnlyCollection<IStorageQueue>>();
            qs.ElementAt(Arg.Any<int>()).Returns(q);
            qs.Count().Returns(i);

            var sqs = new StorageQueueShards(qs);

            await sqs.Save(msg, 0);

            qs.Received().Count();
            qs.Received().ElementAt(i);
            await q.Received().Save(msg);
        }
    }
}