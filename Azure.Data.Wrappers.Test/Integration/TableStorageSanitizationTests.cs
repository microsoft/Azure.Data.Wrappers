namespace Azure.Data.Wrappers.Test.Integration
{
    using Azure.Data.Wrappers;
    using Azure.Data.Wrappers.Sanitization;
    using Azure.Data.Wrappers.Sanitization.Providers;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    [Category("Integration")]
    public class TableStorageSanitizationTests : TableStorageTestsBase
    {
        private string unsanitizedPartition = "#partition#";
        private string unsanitizedRow = "%row%";
        private string sanitizedPartition = "partition";
        private string sanitizedRow = "row";

        private string stubSanitizedValue = "sanitized";

        [OneTimeSetUp]
        public void Init()
        {
            var table = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            this.storage = new TableStorage(table, TestHelpers.DevConnectionString);
            storage.CreateIfNotExists().Wait();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            storage.Delete().Wait();
        }

        #region InsertOrReplaceWithITableEntity
        [Test]
        public void InsertOrReplace_TableEntity_SanitizationRequired_Throws()
        {
            ITableEntity entity = GenerateEntry(unsanitizedPartition, unsanitizedRow); ;
            Assert.ThrowsAsync<StorageException>(async() => await storage.InsertOrReplace(entity));
        }
        [Test]
        public async Task InsertOrReplace_TableEntity_SanitizationRequired()
        {
            ITableEntity entity = GenerateEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            await InsertOrReplace(unsanitizedPartition, unsanitizedRow, sanitizedPartition, sanitizedRow, entity, new DefaultSanitizationProvider());
        }
        #endregion

        #region InsertOrReplaceWithISupportsSanitizedKeys
        [Test]
        public async Task InsertOrReplace_SanitizedKeysTableEntity_NoSanitizationRequired()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: sanitizedPartition, rowKey: sanitizedRow);
            await InsertOrReplace(sanitizedPartition, sanitizedRow, sanitizedPartition, sanitizedRow, entity, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertOrReplace_SanitizedKeysTableEntity_SanitizationRequired()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            await InsertOrReplace(unsanitizedPartition, unsanitizedRow, sanitizedPartition, sanitizedRow, entity, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertOrReplace_SanitizedKeysTableEntity_SanitizationRequired_UsingCustomSanitizationProvider()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            await InsertOrReplace(unsanitizedPartition, unsanitizedRow, stubSanitizedValue, stubSanitizedValue, entity, new StubSanitizationProvider());
        }
        #endregion

        #region InsertOrReplaceWithDictionary
        [Test]
        public void InsertOrReplaceWithDictionary_NoSanitizationProvider_SanitizationRequired_Throws()
        {
            IDictionary<string, object> entities = GenerateDictionaryList(unsanitizedPartition, unsanitizedRow); ;
            Assert.ThrowsAsync<StorageException>(async () => await storage.InsertOrReplace(entities));
        }
        [Test]
        public async Task InsertOrReplaceWithDictionary_SanitizationProvider_SanitizationNotRequired()
        {
            IDictionary<string, object> entities = GenerateDictionaryList(sanitizedPartition, sanitizedRow);
            await InsertOrReplaceDictionary(sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertOrReplaceWithDictionary_SanitizationProvider_SanitizationRequired()
        {
            IDictionary<string, object> entities = GenerateDictionaryList(unsanitizedPartition, unsanitizedRow);
            await InsertOrReplaceDictionary(sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertOrReplaceWithDictionary_SanitizationProvider_SanitizationRequired_UsingCustomSanitizationProvider()
        {
            IDictionary<string, object> entities = GenerateDictionaryList(unsanitizedPartition, unsanitizedRow);
            await InsertOrReplaceDictionary(stubSanitizedValue, stubSanitizedValue, entities, new StubSanitizationProvider());
        }
        #endregion

        #region InsertUsingITableEntity
        [Test]
        public void Insert_TableEntity_NeedsSanitization_Throws()
        {
            ITableEntity entity = GenerateEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            var entities = new List<ITableEntity>();
            entities.Add(entity);

            Assert.ThrowsAsync<StorageException>(async () => await storage.Insert(entities));
        }
        #endregion

        #region InsertUsingISupportsSanitizedKeys
        [Test]
        public async Task Insert_SanitizedKeysTableEntity_DoesNotNeedSanitization()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: sanitizedPartition, rowKey: sanitizedRow);
            var entities = new List<ISupportsSanitizedKeys>() { entity };
            await Insert(sanitizedPartition, sanitizedRow, sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task Insert_SanitizedKeysTableEntity_NeedsSanitization()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            var entities = new List<ISupportsSanitizedKeys>() { entity };
            await Insert(unsanitizedPartition, unsanitizedRow, sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task Insert_SanitizedKeysTableEntity_NeedsSanitization_CustomSanitizationProvider()
        {
            ISupportsSanitizedKeys entity = GenerateSanitizedKeysTableEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            var entities = new List<ISupportsSanitizedKeys>() { entity };
            await Insert(unsanitizedPartition, unsanitizedRow, stubSanitizedValue, stubSanitizedValue, entities, new StubSanitizationProvider());
        }
        #endregion

        #region InsertUsingDyanamicTableEntity
        [Test]
        public async Task Insert_DynamicTableEntity_NeedsSanitization_DefaultSanitizationProvider()
        {
            DynamicTableEntity entity = GenerateDynamicTableEntry(partitionKey: unsanitizedPartition, rowKey: unsanitizedRow);
            var entities = new List<DynamicTableEntity>() { entity };
            await Insert(unsanitizedPartition, unsanitizedRow, sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        #endregion

        #region InsertWithDictionary
        [Test]
        public void InsertWithDictionary_NoSanitizationProvider_SanitizationRequired_Throws()
        {
            IEnumerable<IDictionary<string, object>> entities = new List<IDictionary<string, object>>()
            {
                GenerateDictionaryList(unsanitizedPartition, unsanitizedRow)
            };
            Assert.ThrowsAsync<StorageException>(async () => await storage.Insert(entities));
        }
        [Test]
        public async Task InsertWithDictionary_SanitizationProvider_SanitizationNotRequired()
        {
            IEnumerable<IDictionary<string, object>> entities = new List<IDictionary<string, object>>()
            {
                GenerateDictionaryList(sanitizedPartition, sanitizedRow)
            };

            await InsertDictionary(sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertWithDictionary_SanitizationProvider_SanitizationRequired()
        {
            IEnumerable<IDictionary<string, object>> entities = new List<IDictionary<string, object>>()
            {
                GenerateDictionaryList(unsanitizedPartition, unsanitizedRow)
            };

            await InsertDictionary(sanitizedPartition, sanitizedRow, entities, new DefaultSanitizationProvider());
        }
        [Test]
        public async Task InsertWithDictionary_SanitizationProvider_SanitizationRequired_UsingCustomSanitizationProvider()
        {
            IEnumerable<IDictionary<string, object>> entities = new List<IDictionary<string, object>>()
            {
                GenerateDictionaryList(unsanitizedPartition, unsanitizedRow)
            };

            await InsertDictionary(stubSanitizedValue, stubSanitizedValue, entities, new StubSanitizationProvider());
        }
        #endregion
    }

    public class StubSanitizationProvider : ISanitizationProvider
    {
        public virtual string Sanitize(string input)
        {
            return "sanitized";
        }
    }
}