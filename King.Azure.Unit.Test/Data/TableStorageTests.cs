﻿namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;

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
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new TableStorage(null, ConnectionString);
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
    }
}