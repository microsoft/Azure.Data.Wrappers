namespace Azure.Data.Wrappers.Test.Integration
{
    using Azure.Data.Wrappers.Sanitization;
    using Microsoft.WindowsAzure.Storage.Table;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TableStorageTestsBase
    {
        #region Members
        protected ITableStorage storage = null;
        #endregion

        protected class Helper : TableEntity
        {
            public Guid Id
            {
                get;
                set;
            }
        }

        protected TableEntity GenerateEntry(string partitionKey = null, string rowKey = null)
        {
            if (partitionKey == null) partitionKey = "partition";
            if (rowKey == null) rowKey = "row";

            return new TableEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
            };
        }

        protected SanitizedKeysTableEntity GenerateSanitizedKeysTableEntry(string partitionKey = null, string rowKey = null)
        {
            if (partitionKey == null) partitionKey = "partition";
            if (rowKey == null) rowKey = "row";

            return new SanitizedKeysTableEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
            };
        }

        protected DynamicTableEntity GenerateDynamicTableEntry(string partitionKey = null, string rowKey = null)
        {
            if (partitionKey == null) partitionKey = "partition";
            if (rowKey == null) rowKey = "row";

            return new DynamicTableEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
            };
        }

        protected IDictionary<string, object> GenerateDictionaryList(string partitionKey = null, string rowKey = null)
        {
            if (partitionKey == null) partitionKey = "partition";
            if (rowKey == null) rowKey = "row";

            var dic = new Dictionary<string, object>();
            dic.Add(TableStorage.PartitionKey, partitionKey);
            dic.Add(TableStorage.RowKey, rowKey);
            return dic;
        }

        protected void DeleteTestEntity(ITableEntity entity)
        {
            storage.Delete(entity);
        }

        protected async Task InsertOrReplace(string inputPartition, string inputRow, string sanitizedPartition, string sanitizedRow, ISupportsSanitizedKeys entity, ISanitizationProvider sanitizationProvider = null)
        {
            await storage.InsertOrReplace(entity, sanitizationProvider);

            var returned = await storage.QueryByPartition<TableEntity>(sanitizedPartition);
            var e = returned.First();
            Assert.AreEqual(sanitizedPartition, e.PartitionKey);
            Assert.AreEqual(sanitizedRow, e.RowKey);
            Assert.AreEqual(inputPartition, entity.PartitionKeyUnsanitized);
            Assert.AreEqual(inputRow, entity.RowKeyUnsanitized);
            DeleteTestEntity(entity);
        }

        protected async Task InsertOrReplaceDictionary(string sanitizedPartition, string sanitizedRow, IDictionary<string, object> dictionary, ISanitizationProvider sanitizationProvider = null)
        {
            await storage.InsertOrReplace(dictionary, sanitizationProvider);

            var e = await storage.QueryByPartitionAndRow<Helper>(sanitizedPartition, sanitizedRow);
            Assert.IsNotNull(e);
            Assert.AreEqual(sanitizedPartition, e.PartitionKey);
            Assert.AreEqual(sanitizedRow, e.RowKey);
            await storage.DeleteByPartition(sanitizedPartition);
        }
        protected async Task InsertDictionary(string sanitizedPartition, string sanitizedRow, IEnumerable<IDictionary<string, object>> dictionary, ISanitizationProvider sanitizationProvider = null)
        {
            await storage.Insert(dictionary, sanitizationProvider);

            var e = await storage.QueryByPartitionAndRow<Helper>(sanitizedPartition, sanitizedRow);
            Assert.IsNotNull(e);
            Assert.AreEqual(sanitizedPartition, e.PartitionKey);
            Assert.AreEqual(sanitizedRow, e.RowKey);
            await storage.DeleteByPartition(sanitizedPartition);
        }

        protected async Task Insert(string inputPartition, string inputRow, string sanitizedPartition, string sanitizedRow, List<ISupportsSanitizedKeys> entities, ISanitizationProvider sanitizationProvider = null)
        {
            await storage.Insert(entities, sanitizationProvider);

            var returned = await storage.QueryByPartition<TableEntity>(sanitizedPartition);
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.First();
            Assert.AreEqual(sanitizedPartition, e.PartitionKey);
            Assert.AreEqual(sanitizedRow, e.RowKey);
            Assert.AreEqual(inputPartition, entities.First().PartitionKeyUnsanitized);
            Assert.AreEqual(inputRow, entities.First().RowKeyUnsanitized);
            DeleteTestEntity(entities.First());
        }

        protected async Task Insert(string inputPartition, string inputRow, string sanitizedPartition, string sanitizedRow, List<DynamicTableEntity> entities, ISanitizationProvider sanitizationProvider = null)
        {
            await storage.Insert(entities, sanitizationProvider);

            var returned = await storage.QueryByPartition<TableEntity>(sanitizedPartition);
            Assert.IsNotNull(returned);
            Assert.AreEqual(1, returned.Count());
            var e = returned.First();
            Assert.AreEqual(sanitizedPartition, e.PartitionKey);
            Assert.AreEqual(sanitizedRow, e.RowKey);
            DeleteTestEntity(entities.First());
        }

    }
}
