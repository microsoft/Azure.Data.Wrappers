namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Table Storage
    /// </summary>
    public class TableStorage : AzureStorage, ITableStorage
    {
        #region Members
        /// <summary>
        /// Partition Key
        /// </summary>
        public const string PartitionKey = "PartitionKey";

        /// <summary>
        /// Row Key
        /// </summary>
        public const string RowKey = "RowKey";

        /// <summary>
        /// ETag
        /// </summary>
        public const string ETag = "ETag";

        /// <summary>
        /// Table Client
        /// </summary>
        private readonly CloudTableClient client;

        /// <summary>
        /// Table
        /// </summary>
        private readonly CloudTable reference;
        #endregion

        #region Constructors
        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        public TableStorage(string tableName, string connectionStringKey)
            : base(connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("tableName");
            }

            this.client = base.Account.CreateCloudTableClient();
            this.reference = client.GetTableReference(tableName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Table Name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.reference.Name;
            }
        }

        /// <summary>
        /// Table Client
        /// </summary>
        public virtual CloudTableClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// Table
        /// </summary>
        public virtual CloudTable Reference
        {
            get
            {
                return this.reference;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            return await this.reference.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        public virtual async Task<bool> Create()
        {
            return await this.reference.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Delete Table
        /// </summary>
        /// <param name="tableName"></param>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync();
        }

        /// <summary>
        /// Insert or update the record in table
        /// </summary>
        /// <param name="item">Scheduled Task Entry</param>
        public virtual async Task<TableResult> InsertOrReplace(ITableEntity entry)
        {
            return await this.reference.ExecuteAsync(TableOperation.InsertOrReplace(entry));
        }

        /// <summary>
        /// Insert Or Replace Entity (Dictionary)
        /// </summary>
        /// <remarks>
        /// Specify: PartitionKey, RowKey and ETag
        /// </remarks>
        /// <param name="entity">Entity</param>
        /// <returns>Result</returns>
        public virtual async Task<TableResult> InsertOrReplace(IDictionary<string, object> entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("data");
            }
            
            var properties = new Dictionary<string, EntityProperty>();
            foreach (var key in entity.Keys.Where(k => k != PartitionKey && k != RowKey && k != ETag))
            {
                properties.Add(key, EntityProperty.CreateEntityPropertyFromObject(entity[key]));
            }

            var partitionKey = entity.Keys.Contains(PartitionKey) ? entity[PartitionKey].ToString() : string.Empty;
            var rowKey = entity.Keys.Contains(RowKey) ? entity[RowKey].ToString() : string.Empty;
            var etag = entity.Keys.Contains(ETag) ? entity[ETag].ToString() : null;
            var dynamicEntity = new DynamicTableEntity(partitionKey, rowKey, etag, properties);

            return await this.InsertOrReplace(dynamicEntity);
        }

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities"></param>
        public virtual async Task<IEnumerable<TableResult>> Insert(IEnumerable<ITableEntity> entities)
        {
            var batchOperation = new TableBatchOperation();
            foreach (var entity in entities)
            {
                batchOperation.InsertOrMerge(entity);
            }

            return await this.reference.ExecuteBatchAsync(batchOperation);
        }

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey"></param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<T>> QueryByPartition<T>(string partitionKey)
            where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey));
            return await this.Query<T>(query);
        }

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <remarks>
        /// Without providing the partion this query may not perform well.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<T>> QueryByRow<T>(string rowKey)
            where T : ITableEntity, new()
        {
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey));
            return await this.Query<T>(query);
        }

        /// <summary>
        /// Query By Partition and Row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row</param>
        /// <returns></returns>
        public virtual async Task<T> QueryByPartitionAndRow<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new()
        {
            var partitionFilter = TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, partitionKey);
            var rowFilter = TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey);
            var filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);
            var query = new TableQuery<T>().Where(filter);

            var result = await this.Query<T>(query);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Query
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">Table Query</param>
        /// <returns>Results</returns>
        public virtual async Task<IEnumerable<T>> Query<T>(TableQuery<T> query)
            where T : ITableEntity, new()
        {
            if (null == query)
            {
                throw new ArgumentNullException("query");
            }
            
            var entities = new List<T>();
            TableContinuationToken token = null;

            do
            {
                var queryResult = await this.reference.ExecuteQuerySegmentedAsync<T>(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (null != token);

            return entities;
        }

        /// <summary>
        /// Generic Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<DynamicTableEntity>> Query(TableQuery query)
        {
            if (null == query)
            {
                throw new ArgumentNullException("query");
            }

            var entities = new List<DynamicTableEntity>();
            TableContinuationToken token = null;
            
            do
            {
                var queryResult = await this.reference.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (null != token);

            return entities;
        }

        /// <summary>
        /// Delete By Partition
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <returns>Task</returns>
        public virtual async Task DeleteByPartition(string partitionKey)
        {
            var entities = await this.QueryByPartition<TableEntity>(partitionKey);
            if (null != entities && entities.Any())
            {
                var batchOperation = new TableBatchOperation();
                foreach (var entity in entities)
                {
                    batchOperation.Delete(entity);
                }

                await this.reference.ExecuteBatchAsync(batchOperation);
            }
        }

        /// <summary>
        /// Delete By Row
        /// </summary>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Task</returns>
        public virtual async Task DeleteByRow(string rowKey)
        {
            var entities = await this.QueryByRow<TableEntity>(rowKey);
            if (null != entities && entities.Any())
            {
                foreach (var entity in entities)
                {
                    await this.reference.ExecuteAsync(TableOperation.Delete(entity));
                }
            }
        }

        /// <summary>
        /// Delete By Partition and Row 
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Task</returns>
        public virtual async Task DeleteByPartitionAndRow(string partitionKey, string rowKey)
        {
            var entity = await this.QueryByPartitionAndRow<TableEntity>(partitionKey, rowKey);
            if (null != entity)
            {
                await this.reference.ExecuteAsync(TableOperation.Delete(entity));
            }
        }
        #endregion
    }
}