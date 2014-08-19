namespace King.Service.Integration
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class TableStorageTests
    {
        #region Members
        private readonly string ConnectionString = "UseDevelopmentStorage=true;";
        private ITableStorage storage = null;
        #endregion

        public class Helper : TableEntity
        {
            public Guid Id
            {
                get;
                set;
            }
        }

        [SetUp]
        public void Init()
        {
            var table = "testing";
            this.storage = new TableStorage(table, ConnectionString);
            storage.CreateIfNotExists().Wait();
        }
        
        [TearDown]
        public void Dispose()
        {
            storage.Delete().Wait();
        }

        [Test]
        public async Task CreateIfNotExists()
        {
            var table = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(table, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);

            await storage.Delete();
        }

        [Test]
        public async Task CreateIfNotExistsAlreadyExists()
        {
            var table = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(table, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
            created = await storage.CreateIfNotExists();
            Assert.IsFalse(created);

            await storage.Delete();
        }

        [Test]
        public async Task Create()
        {
            var table = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(table, ConnectionString);
            var created = await storage.Create();

            Assert.IsTrue(created);

            await storage.Delete();
        }

        [Test]
        public async Task Delete()
        {
            var table = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new TableStorage(table, ConnectionString);
            var created = await storage.Create();

            Assert.IsTrue(created);

            await storage.Delete();

            created = await storage.Create();
            Assert.IsTrue(created);

            await storage.Delete();
        }

        [Test]
        public async Task Insert()
        {
            var entity = new TableEntity()
            {
                PartitionKey = "partition",
                RowKey = "row",
            };
            var entities = new List<TableEntity>();
            entities.Add(entity);
            await storage.Insert(entities);

            var returned = storage.QueryByPartition<TableEntity>("partition");
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.First();
            Assert.AreEqual(entity.PartitionKey, e.PartitionKey);
            Assert.AreEqual(entity.RowKey, e.RowKey);
        }

        [Test]
        public async Task InsertOrReplace()
        {
            var entity = new TableEntity()
            {
                PartitionKey = "partition",
                RowKey = "row",
            };
            var entities = new List<TableEntity>();
            entities.Add(entity);
            await storage.Insert(entities);
            await storage.InsertOrReplace(entity);

            var returned = storage.QueryByPartition<TableEntity>("partition");
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.First();
            Assert.AreEqual(entity.PartitionKey, e.PartitionKey);
            Assert.AreEqual(entity.RowKey, e.RowKey);
        }

        [Test]
        public async Task QueryByPartition()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<Helper>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var h = new Helper()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                };
                entities.Add(h);
            }

            await storage.Insert(entities);

            var returned = storage.QueryByPartition<Helper>(partition);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where e.RowKey == r.RowKey
                              && e.Id == r.Id
                              select true).FirstOrDefault();
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryByRow()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<Helper>();
            var rowKey = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var h = new Helper()
                {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = rowKey,
                    Id = Guid.NewGuid(),
                };
                entities.Add(h);

                await storage.InsertOrReplace(h);
            }

            var returned = storage.QueryByRow<Helper>(rowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where e.RowKey == r.RowKey
                              && e.Id == r.Id
                              select true).FirstOrDefault();
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryByPartitionAndRow()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<Helper>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var h = new Helper()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                };
                entities.Add(h);
            }

            var z = new Helper()
            {
                PartitionKey = partition,
                RowKey = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid(),
            };
            entities.Add(z);

            await storage.Insert(entities);

            var returned = storage.QueryByPartitionAndRow<Helper>(z.PartitionKey, z.RowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual(z.Id, returned.Id);
        }

        [Test]
        public async Task DeleteByPartition()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<Helper>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var h = new Helper()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                };
                entities.Add(h);
            }

            await storage.Insert(entities);
            await storage.DeleteByPartition(partition);

            var returned = storage.QueryByPartition<Helper>(partition);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }

        [Test]
        public async Task DeleteByPartitionPartitionNull()
        {
            await storage.DeleteByPartition(null);
        }

        [Test]
        public void QueryByPartitionPartitionNull()
        {
            var returned = storage.QueryByPartition<Helper>(null);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }
    }
}
