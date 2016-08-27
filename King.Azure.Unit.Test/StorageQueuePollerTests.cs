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
        public void ConstructorStorageQueueNull()
        {
            Assert.That(() => new StorageQueuePoller<object>(null), Throws.TypeOf<ArgumentNullException>());
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

            await queue.Received().Get();
        }

        [Test]
        public async Task PollGetNull()
        {
            var queue = Substitute.For<IStorageQueue>();
            queue.Get().Returns(Task.FromResult<CloudQueueMessage>(null));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.Poll();

            Assert.IsNull(returned);

            await queue.Received().Get();
        }

        [Test]
        public void PollGetThrows()
        {
            var msg = new CloudQueueMessage("data");
            var queue = Substitute.For<IStorageQueue>();
            queue.Get().ReturnsForAnyArgs<object>(x => { throw new ApplicationException(); });

            var poller = new StorageQueuePoller<object>(queue);

            Assert.That(() => poller.Poll(), Throws.TypeOf<ApplicationException>());
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

            await queue.Received().GetMany(3);
        }

        [Test]
        public async Task PollGetManyNull()
        {
            var queue = Substitute.For<IStorageQueue>();
            queue.GetMany(3).Returns(Task.FromResult<IEnumerable<CloudQueueMessage>>(null));

            var poller = new StorageQueuePoller<object>(queue);
            var returned = await poller.PollMany(3);

            Assert.IsNull(returned);

            await queue.Received().GetMany(3);
        }

        [Test]
        public void PollGetManyThrows()
        {
            var msg = new CloudQueueMessage("data");
            var queue = Substitute.For<IStorageQueue>();
            queue.GetMany().ReturnsForAnyArgs<object>(x => { throw new ApplicationException(); });

            var poller = new StorageQueuePoller<object>(queue);

            Assert.That(() => poller.PollMany(), Throws.TypeOf<ApplicationException>());
        }
    }
}