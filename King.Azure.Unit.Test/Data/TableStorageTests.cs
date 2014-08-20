namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TableStorageTests
    {
        [Test]
        public void Constructor()
        {
            new TableStorage("TestTable", "UseDevelopmentStorage=true");
        }

        [Test]
        public void IsITableStorage()
        {
            Assert.IsNotNull(new TableStorage("TestTable", "UseDevelopmentStorage=true") as ITableStorage);
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
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new TableStorage(null, "UseDevelopmentStorage=true");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorKeyNull()
        {
            new TableStorage("TestTable", null);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, "UseDevelopmentStorage=true");
            Assert.AreEqual(name, t.Name);
        }
    }
}