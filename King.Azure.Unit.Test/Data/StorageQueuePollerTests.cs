namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class StorageQueuePollerTests
    {
        const string ConnectionString = "UseDevelopmentStorage=true";

        [Test]
        public void Constructor()
        {
            new StorageQueuePoller<object>("queue", ConnectionString);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorStorageQueueNull()
        {
            new StorageQueuePoller<object>(null);
        }

        [Test]
        public void IsIStorageQueuePoller()
        {
            Assert.IsNotNull(new StorageQueuePoller<object>("queue", ConnectionString) as IStorageQueuePoller<object>);
        }

        [Test]
        public void Queue()
        {
            var queue = Substitute.For<IStorageQueue>();
            var poller = new StorageQueuePoller<object>(queue);
            var returned = poller.Queue;
            Assert.AreEqual(queue, returned);
        }

        [Test]
        public async Task Poll()
        {
            var msg = new CloudQueueMessage("data");
            var queue = Substitute.For<IStorageQueue>();
            queue.Get().Returns(Task.FromResult(msg));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.Poll();

            Assert.IsNotNull(returned);

            queue.Received().Get();
        }

        [Test]
        public async Task PollGetNull()
        {
            var queue = Substitute.For<IStorageQueue>();
            queue.Get().Returns(Task.FromResult<CloudQueueMessage>(null));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.Poll();

            Assert.IsNull(returned);

            queue.Received().Get();
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public async Task PollGetThrows()
        {
            var msg = new CloudQueueMessage("data");
            var queue = Substitute.For<IStorageQueue>();
            queue.Get().Returns(x => { throw new ApplicationException(); });

            var poller = new StorageQueuePoller<object>(queue);
            await poller.Poll();
        }

        [Test]
        public async Task PollMany()
        {
            var msg = new CloudQueueMessage("data");
            var msgs = new List<CloudQueueMessage>(3);
            msgs.Add(msg);
            msgs.Add(msg);
            msgs.Add(msg);

            var queue = Substitute.For<IStorageQueue>();
            queue.GetMany(3).Returns(Task.FromResult<IEnumerable<CloudQueueMessage>>(msgs));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.PollMany(3);

            Assert.IsNotNull(returned);
            Assert.AreEqual(3, returned.Count());

            queue.Received().GetMany(3);
        }

        [Test]
        public async Task PollGetManyNull()
        {
            var queue = Substitute.For<IStorageQueue>();
            queue.GetMany(3).Returns(Task.FromResult<IEnumerable<CloudQueueMessage>>(null));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.PollMany(3);

            Assert.IsNull(returned);

            queue.Received().GetMany(3);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public async Task PollGetManyThrows()
        {
            var msg = new CloudQueueMessage("data");
            var queue = Substitute.For<IStorageQueue>();
            queue.GetMany().Returns(x => { throw new ApplicationException(); });

            var poller = new StorageQueuePoller<object>(queue);
            await poller.PollMany();
        }
    }
}