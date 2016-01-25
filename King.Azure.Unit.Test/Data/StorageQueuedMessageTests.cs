namespace King.Azure.Unit.Test.Data
{
    using System;
    using System.Threading.Tasks;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StorageQueuedMessageTests
    {
        public class Helper
        {
            public Guid Test
            {
                get;
                set;
            }
        }

        [Test]
        public void Constructor()
        {
            var queue = Substitute.For<IStorageQueue>();
            new StorageQueuedMessage<object>(queue, new CloudQueueMessage("ship"));
        }

        [Test]
        public void ConstructorQueueNull()
        {
            var message = new CloudQueueMessage("ship");

            Assert.That(() => new StorageQueuedMessage<object>(null, message), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructorMessageNull()
        {
            var queue = Substitute.For<IStorageQueue>();

            Assert.That(() => new StorageQueuedMessage<object>(queue, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task Complete()
        {
            var queue = Substitute.For<IStorageQueue>();
            var message = new CloudQueueMessage("ship");
            await queue.Delete(message);
            
            var sqm = new StorageQueuedMessage<object>(queue, message);
            await sqm.Complete();

            await queue.Received().Delete(message);
        }

        [Test]
        public async Task Abandon()
        {
            var queue = Substitute.For<IStorageQueue>();
            var message = new CloudQueueMessage("ship");

            var sqm = new StorageQueuedMessage<object>(queue, message);
            await sqm.Abandon();
        }

        [Test]
        public async Task Data()
        {
            var expected = new Helper()
            {
                Test = Guid.NewGuid(),
            };
            var json = JsonConvert.SerializeObject(expected);
            var queue = Substitute.For<IStorageQueue>();
            var message = new CloudQueueMessage(json);

            var sqm = new StorageQueuedMessage<Helper>(queue, message);
            var data = await sqm.Data();

            Assert.IsNotNull(data);
            Assert.AreEqual(expected.Test, data.Test);
        }
    }
}