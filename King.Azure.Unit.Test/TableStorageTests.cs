namespace King.Azure.Unit.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;

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
        public void ConstructorTableNull()
        {
            Assert.That(() => new TableStorage(null, ConnectionString), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorAccountTableNull()
        {
            Assert.That(() => new TableStorage(null, CloudStorageAccount.Parse(ConnectionString)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorConnectionStringNull()
        {
            Assert.That(() => new TableStorage("TestTable", (string)null), Throws.TypeOf<ArgumentNullException>());
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
        public void InsertDictionaryNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.InsertOrReplace((IDictionary<string, object>)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void QueryFunctionNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Query<TableEntity>(null, 1000), Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void QueryFunctionResultsNegative()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Query<TableEntity>(i => i.PartitionKey == "hi", -100), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void QueryTableQueryNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Query<TableEntity>(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void QueryDictionaryQueryNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Query((TableQuery)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DeleteEntityNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Delete((ITableEntity)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DeleteEntitiesNull()
        {
            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            Assert.That(() => t.Delete((IEnumerable<ITableEntity>)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void BatchOne()
        {
            var items = new List<ITableEntity>();
            items.Add(new TableEntity());

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(1, batches.Count());
            Assert.AreEqual(1, batches.First().Count());
        }

        [Test]
        public void BatchNone()
        {
            var items = new List<ITableEntity>();

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(0, batches.Count());
        }

        [Test]
        public void BatchThousandsDifferentPartitions()
        {
            var random = new Random();
            var count = random.Next(2001, 10000);
            var items = new List<ITableEntity>();

            for (var i = 0; i < count; i++)
            {
                items.Add(new TableEntity() { PartitionKey = Guid.NewGuid().ToString() });
            }

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(count, batches.Count());
        }

        [Test]
        public void BatchThousands()
        {
            var random = new Random();
            var count = random.Next(2001, 10000);
            var partition = Guid.NewGuid().ToString();
            var items = new List<ITableEntity>();

            for (var i = 0; i < count; i++)
            {
                items.Add(new TableEntity() { PartitionKey = partition });
            }

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(Math.Ceiling(((double)count / TableStorage.MaimumxInsertBatch)), batches.Count());
            var resultCount = 0;
            foreach (var b in batches)
            {
                resultCount += b.Count();
            }
            Assert.AreEqual(count, resultCount);
        }

        [Test]
        public void ChunkThousands()
        {
            var random = new Random();
            var count = random.Next(2001, 15000);
            var partition = Guid.NewGuid().ToString();
            var items = new List<ITableEntity>();

            for (var i = 0; i < count; i++)
            {
                items.Add(new TableEntity() { PartitionKey = partition });
            }

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Chunk<ITableEntity>(items);
            Assert.AreEqual(Math.Ceiling(((double)count / TableStorage.MaimumxInsertBatch)), batches.Count());
            var resultCount = 0;
            foreach (var b in batches)
            {
                resultCount += b.Count();
            }
            Assert.AreEqual(count, resultCount);
        }

        [Test]
        public void ChunkOne()
        {
            var items = new List<ITableEntity>();
            items.Add(new TableEntity());

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Chunk<ITableEntity>(items);
            Assert.AreEqual(1, batches.Count());
            Assert.AreEqual(1, batches.First().Count());
        }

        [Test]
        public void ChunkNone()
        {
            var items = new List<ITableEntity>();

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Chunk<ITableEntity>(items);
            Assert.AreEqual(0, batches.Count());
        }

        [Test]
        public void BatchDictionaryOne()
        {
            var items = new List<IDictionary<string, object>>();
            var dic = new Dictionary<string, object>();
            dic.Add(TableStorage.PartitionKey, Guid.NewGuid().ToString());
            items.Add(dic);

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(1, batches.Count());
            Assert.AreEqual(1, batches.First().Count());
        }

        [Test]
        public void BatchDictionaryNone()
        {
            var items = new List<IDictionary<string, object>>();

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(0, batches.Count());
        }

        [Test]
        public void BatchDictionaryThousandsDifferentPartitions()
        {
            var random = new Random();
            var count = random.Next(2001, 10000);
            var items = new List<IDictionary<string, object>>();

            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, Guid.NewGuid().ToString());
                items.Add(dic);
            }

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(count, batches.Count());
        }

        [Test]
        public void BatchDictionaryThousands()
        {
            var random = new Random();
            var count = random.Next(2001, 10000);
            var partition = Guid.NewGuid().ToString();
            var items = new List<IDictionary<string, object>>();

            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                items.Add(dic);
            }

            var name = Guid.NewGuid().ToString();
            var t = new TableStorage(name, ConnectionString);

            var batches = t.Batch(items);
            Assert.AreEqual(Math.Ceiling(((double)count / TableStorage.MaimumxInsertBatch)), batches.Count());
            var resultCount = 0;
            foreach (var b in batches)
            {
                resultCount += b.Count();
            }
            Assert.AreEqual(count, resultCount);
        }
    }
}