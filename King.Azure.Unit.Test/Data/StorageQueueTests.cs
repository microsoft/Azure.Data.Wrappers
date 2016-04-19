namespace King.Azure.Unit.Test.Data
{
    using System;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;

    [TestFixture]
    public class StorageQueueTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new StorageQueue("test", ConnectionString, TimeSpan.FromSeconds(22));
        }

        [Test]
        public void IQueue()
        {
            Assert.IsNotNull(new StorageQueue("test", ConnectionString) as IStorageQueue);
        }

        [Test]
        public void ConstructorTableNull()
        {
            Assert.That(() => new StorageQueue(null, ConnectionString), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorAccountTableNull()
        {
            Assert.That(() => new StorageQueue(null, CloudStorageAccount.Parse(ConnectionString)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorKeyNull()
        {
            Assert.That(() => new StorageQueue("test", (string)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            Assert.AreEqual(name, t.Name);
        }

        [Test]
        public void Client()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            Assert.IsNotNull(t.Client);
        }

        [Test]
        public void Reference()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            Assert.IsNotNull(t.Reference);
        }

        [Test]
        public void DeleteNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);

            Assert.That(() => t.Delete(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SaveMessageNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);

            Assert.That(() => t.Send((CloudQueueMessage)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SaveNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);

            Assert.That(() => t.Send((object)null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}