namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class TableStorageTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new TableStorage("TestTable", ConnectionString);
        }

        [Test]
        public void IsITableStorage()
        {
            Assert.IsNotNull(new TableStorage("TestTable", ConnectionString) as ITableStorage);
        }

        [Test]
        public void PartitionKey()
        {
            Assert.AreEqual("PartitionKey", TableStorage.PartitionKey);
        }

        [Test]
        public void RowKey()
        {
            Assert.AreEqual("RowKey", TableStorage.RowKey);
        }

        [Test]
        public void Timestamp()
        {
            Assert.AreEqual("Timestamp", TableStorage.Timestamp);
        }

        [Test]
        public void ETag()
        {
            Assert.AreEqual("ETag", TableStorage.ETag);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new TableStorage(null, ConnectionString);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorAccountTableNull()
        {
            new TableStorage(null, CloudStorageAccount.Parse(ConnectionString));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorConnectionStringNull()
        {
            new TableStorage("TestTable", (string)null);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            Assert.AreEqual(name, t.Name);
        }

        [Test]
        public void Client()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            Assert.IsNotNull(t.Client);
        }

        [Test]
        public void Reference()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            Assert.IsNotNull(t.Reference);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task InsertDictionaryNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            await t.InsertOrReplace((IDictionary<string, object>)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task QueryTableQueryNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            await t.Query<TableEntity>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task DeleteEntityNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);
            await t.Delete(null);
        }
    }
}