namespace King.Service.Integration
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
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
        public async Task ConstructorAccount()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var account = CloudStorageAccount.Parse(ConnectionString);
            var storage = new TableStorage(name, account);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
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

            var returned = await storage.QueryByPartition<TableEntity>("partition");
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
            await storage.InsertOrReplace(entity);

            var returned = await storage.QueryByPartition<TableEntity>("partition");
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.First();
            Assert.AreEqual(entity.PartitionKey, e.PartitionKey);
            Assert.AreEqual(entity.RowKey, e.RowKey);
        }

        [Test]
        public async Task InsertBatch()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var partition = Guid.NewGuid().ToString();
            var entities = new List<TableEntity>(count);
            for (var i = 0; i < count; i++)
            {
                var entity = new TableEntity()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                };
                entities.Add(entity);
            }
            await storage.Insert(entities);

            var returned = await storage.QueryByPartition<TableEntity>(partition);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
        }

        [Test]
        public async Task InsertBatchMultiplePartitions()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var partition = Guid.NewGuid().ToString();
            var entities = new List<TableEntity>(count);
            for (var i = 0; i < count; i++)
            {
                var entity = new TableEntity()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                };
                entities.Add(entity);

                if (i % 2 == 0)
                {
                    partition = Guid.NewGuid().ToString();
                }
            }
            await storage.Insert(entities);

            var returned = await storage.Query<TableEntity>(new TableQuery<TableEntity>());
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
        }

        [Test]
        public async Task InsertDictionaryBatch()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var partition = Guid.NewGuid().ToString();
            var entities = new List<IDictionary<string, object>>(count);
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid());
                dic.Add("Extraa", DateTime.UtcNow);
                entities.Add(dic);
            }
            await storage.Insert(entities);

            var query = new TableQuery();
            query.Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, partition));
            var returned = await storage.Query(query);

            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
        }

        [Test]
        public async Task InsertDictionaryBatchMultiplePartitions()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var partition = Guid.NewGuid().ToString();
            var entities = new List<IDictionary<string, object>>(count);
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid());
                dic.Add("Extraa", DateTime.UtcNow);
                entities.Add(dic);

                if (i % 2 == 0)
                {
                    partition = Guid.NewGuid().ToString();
                }
            }
            await storage.Insert(entities);

            var returned = await storage.Query(new TableQuery());

            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
        }

        [Test]
        public async Task InsertOrReplaceDictionary()
        {
            var p = Guid.NewGuid().ToString();
            var r = Guid.NewGuid().ToString();
            var entity = new Dictionary<string, object>();
            entity.Add(TableStorage.PartitionKey, p);
            entity.Add(TableStorage.RowKey, r);
            entity.Add("Id", Guid.NewGuid());
            await storage.InsertOrReplace(entity);

            var e = await storage.QueryByPartitionAndRow<Helper>(p, r);
            Assert.IsNotNull(e);
            Assert.AreEqual(entity[TableStorage.PartitionKey], e.PartitionKey);
            Assert.AreEqual(entity[TableStorage.RowKey], e.RowKey);
            Assert.AreEqual(entity["Id"], e.Id);
        }

        [Test]
        public async Task InsertOrReplaceDictionaryPartitionRowGuid()
        {
            var p = Guid.NewGuid();
            var r = Guid.NewGuid();
            var entity = new Dictionary<string, object>();
            entity.Add(TableStorage.PartitionKey, p);
            entity.Add(TableStorage.RowKey, r);
            entity.Add("Id", Guid.NewGuid());
            await storage.InsertOrReplace(entity);

            var e = await storage.QueryByPartitionAndRow<Helper>(p.ToString(), r.ToString());
            Assert.IsNotNull(e);
            Assert.AreEqual(entity[TableStorage.PartitionKey].ToString(), e.PartitionKey);
            Assert.AreEqual(entity[TableStorage.RowKey].ToString(), e.RowKey);
            Assert.AreEqual(entity["Id"], e.Id);
        }

        [Test]
        public async Task InsertOrReplaceDictionaryNoRow()
        {
            var p = Guid.NewGuid().ToString();
            var entity = new Dictionary<string, object>();
            entity.Add(TableStorage.PartitionKey, p);
            await storage.InsertOrReplace(entity);

            var returned = await storage.QueryByPartition<TableEntity>(p);
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.FirstOrDefault();
            Assert.AreEqual(entity[TableStorage.PartitionKey], e.PartitionKey);
        }

        [Test]
        public async Task InsertOrReplaceDictionaryNoPartition()
        {
            var r = Guid.NewGuid().ToString();
            var entity = new Dictionary<string, object>();
            entity.Add(TableStorage.RowKey, r);
            await storage.InsertOrReplace(entity);

            var returned = await storage.QueryByRow<TableEntity>(r);
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.FirstOrDefault();
            Assert.AreEqual(entity[TableStorage.RowKey], e.RowKey);
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

            var returned = await storage.QueryByPartition<Helper>(partition);
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

            var returned = await storage.QueryByRow<Helper>(rowKey);
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

            var returned = await storage.QueryByPartitionAndRow<Helper>(z.PartitionKey, z.RowKey);
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

            var returned = await storage.QueryByPartition<Helper>(partition);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }

        [Test]
        public async Task DeleteByPartitionPartitionNull()
        {
            await storage.DeleteByPartition(null);
        }

        [Test]
        public async Task DeleteByPartitionAndRow()
        {
            var h = new Helper()
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid(),
            };

            await storage.InsertOrReplace(h);
            await storage.DeleteByPartitionAndRow(h.PartitionKey, h.RowKey);

            var returned = await storage.QueryByPartitionAndRow<Helper>(h.PartitionKey, h.RowKey);
            Assert.IsNull(returned);
        }

        [Test]
        public async Task DeleteEnity()
        {
            var h = new Helper()
            {
                PartitionKey = Guid.NewGuid().ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid(),
            };

            await storage.InsertOrReplace(h);
            var result = await storage.Delete(h);
            Assert.IsNotNull(result);

            var returned = await storage.QueryByPartitionAndRow<Helper>(h.PartitionKey, h.RowKey);
            Assert.IsNull(returned);
        }

        [Test]
        public async Task DeleteEnities()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<ITableEntity>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                entities.Add(new Helper()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid(),
                }
                );
            }

            await storage.Insert(entities);

            var result = await storage.Delete(entities);
            Assert.AreEqual(count, result.Count());

            foreach (var e in entities)
            {
                var returned = await storage.QueryByPartitionAndRow<Helper>(e.PartitionKey, e.RowKey);
                Assert.IsNull(returned);
            }
        }

        [Test]
        public async Task DeleteMultipleBatches()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var partition = Guid.NewGuid().ToString();
            var entities = new List<TableEntity>(count);
            for (var i = 0; i < count; i++)
            {
                var entity = new TableEntity()
                {
                    PartitionKey = partition,
                    RowKey = Guid.NewGuid().ToString(),
                };
                entities.Add(entity);

                if (i % 2 == 0)
                {
                    partition = Guid.NewGuid().ToString();
                }
            }

            await storage.Insert(entities);

            var result = await storage.Delete(entities);
            Assert.AreEqual(count, result.Count());

            var returned = await storage.Query(new TableQuery());

            Assert.IsNotNull(returned);
            Assert.AreEqual(0, returned.Count());
        }

        [Test]
        public async Task DeleteEntitiesNone()
        {
            var result = await storage.Delete(new List<TableEntity>());
            Assert.IsNull(result);
        }

        [Test]
        public async Task QueryByPartitionPartitionNull()
        {
            var returned = await storage.QueryByPartition<Helper>(null);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }

        [Test]
        public async Task QueryByRowPartitionNull()
        {
            var returned = await storage.QueryByRow<Helper>(null);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }

        [Test]
        public async Task QueryByPartitionAndRowPartitionNullRowNull()
        {
            var returned = await storage.QueryByPartitionAndRow<Helper>(null, null);
            Assert.IsNull(returned);
        }

        [Test]
        public async Task DeleteByRow()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var rowKey = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var h = new Helper()
                {
                    PartitionKey = Guid.NewGuid().ToString(),
                    RowKey = rowKey,
                    Id = Guid.NewGuid(),
                };
                await storage.InsertOrReplace(h);
            }

            await storage.DeleteByRow(rowKey);

            var returned = await storage.QueryByRow<Helper>(rowKey);
            Assert.IsNotNull(returned);
            Assert.IsFalse(returned.Any());
        }

        [Test]
        public async Task DeleteByRowRowNull()
        {
            await storage.DeleteByRow(null);
        }

        [Test]
        public async Task QueryByPartitionGreaterThan1000()
        {
            var random = new Random();
            var count = random.Next(1001, 1250);
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

            var returned = await storage.QueryByPartition<Helper>(partition);
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
        public async Task QueryGreaterThan1000()
        {
            var random = new Random();
            var count = random.Next(1001, 1250);
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

            var query = new TableQuery();
            query.Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, partition));
            var returned = await storage.Query(query);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where e.RowKey == (string)r[TableStorage.RowKey]
                              select true).FirstOrDefault();

                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task Query()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid().ToString());
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);
            }

            await storage.Insert(entities);

            var query = new TableQuery();
            query.Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, partition));
            var returned = await storage.Query(query);

            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where (string)e[TableStorage.PartitionKey] == (string)r[TableStorage.PartitionKey]
                                && (string)e[TableStorage.RowKey] == (string)r[TableStorage.RowKey]
                                && (Guid)e["Id"] == (Guid)r["Id"]
                                && !string.IsNullOrWhiteSpace((string)r[TableStorage.ETag])
                                && DateTime.UtcNow.Date == ((DateTime)r[TableStorage.Timestamp]).Date
                              select true).FirstOrDefault();
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryFunction()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid().ToString());
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);
            }

            await storage.Insert(entities);
            
            var returned = await storage.Query<Helper>(i => i.PartitionKey == partition);

            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where r.RowKey == (string)e[TableStorage.RowKey]
                                && r.PartitionKey == (string)e[TableStorage.PartitionKey]
                              select true).FirstOrDefault();

                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryFunctionNone()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid().ToString());
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);
            }

            await storage.Insert(entities);

            var returned = await storage.Query<Helper>(i => i.PartitionKey != partition);

            Assert.IsNotNull(returned);
            Assert.AreEqual(0, returned.Count());
        }

        [Test]
        public async Task QueryByPartitionDictionary()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid().ToString());
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);
            }

            await storage.Insert(entities);

            var returned = await storage.QueryByPartition(partition);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where (string)e[TableStorage.RowKey] == (string)r[TableStorage.RowKey]
                                && (Guid)e["Id"] == (Guid)r["Id"]
                              select true).FirstOrDefault();
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryByRowDictionary()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var rowKey = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, Guid.NewGuid().ToString());
                dic.Add(TableStorage.RowKey, rowKey);
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);

                await storage.InsertOrReplace(dic);
            }

            var returned = await storage.QueryByRow(rowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual(count, returned.Count());
            foreach (var r in returned)
            {
                var exists = (from e in entities
                              where (string)e[TableStorage.RowKey] == (string)r[TableStorage.RowKey]
                                && (Guid)e["Id"] == (Guid)r["Id"]
                              select true).FirstOrDefault();
                Assert.IsTrue(exists);
            }
        }

        [Test]
        public async Task QueryByPartitionAndRowDictionary()
        {
            var random = new Random();
            var count = random.Next(1, 25);
            var entities = new List<IDictionary<string, object>>();
            var partition = Guid.NewGuid().ToString();
            for (var i = 0; i < count; i++)
            {
                var dic = new Dictionary<string, object>();
                dic.Add(TableStorage.PartitionKey, partition);
                dic.Add(TableStorage.RowKey, Guid.NewGuid().ToString());
                dic.Add("Id", Guid.NewGuid());
                entities.Add(dic);
            }

            var rowKey = Guid.NewGuid().ToString();
            var z = new Dictionary<string, object>();
            z.Add(TableStorage.PartitionKey, partition);
            z.Add(TableStorage.RowKey, rowKey);
            z.Add("Id", Guid.NewGuid());
            entities.Add(z);

            await storage.Insert(entities);

            var returned = await storage.QueryByPartitionAndRow(partition, rowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual(z["Id"], returned["Id"]);
        }
    }
}