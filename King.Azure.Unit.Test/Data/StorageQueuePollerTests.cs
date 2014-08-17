namespace King.Azure.Unit.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NSubstitute;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage.Queue;

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
    }
}