namespace King.Azure.Unit.Test.Data
{
    using System;
    using System.Threading.Tasks;
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
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new StorageQueue(null, ConnectionString);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorAccountTableNull()
        {
            new StorageQueue(null, CloudStorageAccount.Parse(ConnectionString));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorKeyNull()
        {
            new StorageQueue("test", (string)null);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            await t.Delete(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveMessageNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            await t.Save((CloudQueueMessage)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new StorageQueue(name, ConnectionString);
            await t.Save((object)null);
        }
    }
}