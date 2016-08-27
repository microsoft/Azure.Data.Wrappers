namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            Assert.That(() => new StorageQueueShards(new IStorageQueue[0]), Throws.TypeOf<ArgumentException>());
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
        public void IsIAzureStorage()
        {
            Assert.IsNotNull(new StorageQueueShards("test", ConnectionString) as IAzureStorage);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var sqs = new StorageQueueShards(name, ConnectionString, 2);
            Assert.AreEqual(name, sqs.Name);
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
            var sqs = new StorageQueueShards(qs.ToArray());

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
            var sqs = new StorageQueueShards(qs.ToArray());

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
            var index = random.Next(0, i);

            var msg = new object();
            var qs = new List<IStorageQueue>();

            for (var j = 0; j < i; j++)
            {
                var q = Substitute.For<IStorageQueue>();
                q.Send(msg).Returns(Task.CompletedTask);
                qs.Add(q);
            }

            var sqs = new StorageQueueShards(qs);

            await sqs.Save(msg, (byte)index);

            for (var j = 0; j < i; j++)
            {
                if (j == index)
                {
                    await qs[j].Received().Send(msg);
                }
                else
                {
                    await qs[j].DidNotReceive().Send(msg);
                }
            }
        }

        [Test]
        public void Index()
        {
            var msg = new object();
            var q = Substitute.For<IStorageQueue>();

            var qs = new List<IStorageQueue>();
            qs.Add(q);
            qs.Add(q);
            qs.Add(q);

            var sqs = new StorageQueueShards(qs);

            var index = sqs.Index(0);

            Assert.IsTrue(0 <= index && 3 > index);
        }
        
        [Test]
        public void IndexBad([Values(0,255)] int val, [Values(0,0)] int expected)
        {
            var msg = new object();
            var q = Substitute.For<IStorageQueue>();

            var qs = new List<IStorageQueue>();
            qs.Add(q);

            var sqs = new StorageQueueShards(qs);

            var index = sqs.Index((byte)val);

            Assert.AreEqual(expected, index);
        }
    }
}