namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.WindowsAzure.Storage.Table;
    //using Microsoft.WindowsAzure.Storage.Table.Queryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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
        /// Timestamp
        /// </summary>
        public const string Timestamp = "Timestamp";

        /// <summary>
        /// ETag
        /// </summary>
        public const string ETag = "ETag";

        /// <summary>
        /// Maximum Insert Batch
        /// </summary>
        public const int MaimumxInsertBatch = 100;

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
        /// <param name="connectionString">Connection String</param>
        /// <param name="location">Location Mode</param>
        public TableStorage(string tableName, string connectionString, LocationMode location = LocationMode.PrimaryThenSecondary)
            : this(tableName, CloudStorageAccount.Parse(connectionString), location)
        {
        }

        /// <summary>
        /// Table Storage
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="account">Storage Account</param>
        /// <param name="location">Location Mode</param>
        public TableStorage(string tableName, CloudStorageAccount account, LocationMode location = LocationMode.PrimaryThenSecondary)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException("tableName");
            }

            this.client = base.Account.CreateCloudTableClient();
            this.client.DefaultRequestOptions.LocationMode = location;

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

        #region Create Table
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
        #endregion

        #region Delete
        /// <summary>
        /// Delete Table
        /// </summary>
        /// <param name="tableName"></param>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync();
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
                await this.Delete(entities);
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
                    await this.Delete(entity);
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
                await this.Delete(entity);
            }
        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        public virtual async Task<TableResult> Delete(ITableEntity entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("entity");
            }

            return await this.reference.ExecuteAsync(TableOperation.Delete(entity));
        }

        /// <summary>
        /// Delete Entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Table Results</returns>
        public virtual async Task<IEnumerable<TableResult>> Delete(IEnumerable<ITableEntity> entities)
        {
            if (null == entities)
            {
                throw new ArgumentNullException("entities");
            }
            if (!entities.Any())
            {
                return null;
            }

            var result = new List<TableResult>();

            foreach (var batch in this.Batch(entities))
            {
                var batchOperation = new TableBatchOperation();
                batch.ToList().ForEach(e => batchOperation.Delete(e));
                var r = await this.reference.ExecuteBatchAsync(batchOperation);
                result.AddRange(r);
            }

            return result;
        }
        #endregion

        #region Save Data
        /// <summary>
        /// Insert or update the record in table
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<TableResult> InsertOrReplace(ITableEntity entity)
        {
            return await this.reference.ExecuteAsync(TableOperation.InsertOrReplace(entity));
        }

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<TableResult>> Insert(IEnumerable<ITableEntity> entities)
        {
            var result = new List<TableResult>();

            foreach (var batch in this.Batch(entities))
            {
                var batchOperation = new TableBatchOperation();
                batch.ToList().ForEach(e => batchOperation.InsertOrReplace(e));
                var r = await this.reference.ExecuteBatchAsync(batchOperation);
                result.AddRange(r);
            }

            return result;
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
            entity.Keys.Where(k => k != PartitionKey && k != RowKey && k != ETag).ToList().ForEach(key => properties.Add(key, EntityProperty.CreateEntityPropertyFromObject(entity[key])));

            var partitionKey = entity.Keys.Contains(PartitionKey) ? entity[PartitionKey].ToString() : string.Empty;
            var rowKey = entity.Keys.Contains(RowKey) ? entity[RowKey].ToString() : string.Empty;
            var etag = entity.Keys.Contains(ETag) ? entity[ETag].ToString() : null;
            var dynamicEntity = new DynamicTableEntity(partitionKey, rowKey, etag, properties);

            return await this.InsertOrReplace(dynamicEntity);
        }

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<TableResult>> Insert(IEnumerable<IDictionary<string, object>> entities)
        {
            var result = new List<TableResult>();

            foreach (var batch in this.Batch(entities))
            {
                var batchOperation = new TableBatchOperation();

                foreach (var entity in batch)
                {
                    var properties = new Dictionary<string, EntityProperty>();
                    entity.Keys.Where(k => k != PartitionKey && k != RowKey && k != ETag).ToList().ForEach(key => properties.Add(key, EntityProperty.CreateEntityPropertyFromObject(entity[key])));

                    var partitionKey = entity.Keys.Contains(PartitionKey) ? entity[PartitionKey].ToString() : string.Empty;
                    var rowKey = entity.Keys.Contains(RowKey) ? entity[RowKey].ToString() : string.Empty;
                    var etag = entity.Keys.Contains(ETag) ? entity[ETag].ToString() : null;

                    batchOperation.InsertOrMerge(new DynamicTableEntity(partitionKey, rowKey, etag, properties));
                }

                var r = await this.reference.ExecuteBatchAsync(batchOperation);
                result.AddRange(r);
            }

            return result;
        }
        #endregion

        #region Query Object
        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
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
        /// <typeparam name="T">Return Type</typeparam>
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
        /// <typeparam name="T">Return Type</typeparam>
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
        /// Query by Expression
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="maxResults">Max Result</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> Query<T>(Expression<Func<T, bool>> predicate, int maxResults = int.MaxValue)
            where T : ITableEntity, new()
        {
            if (null == predicate)
            {
                throw new ArgumentNullException("predicate");
            }
            if (0 >= maxResults)
            {
                throw new InvalidOperationException("maxResults: must be above 0.");
            }

            //var query = this.reference.CreateQuery<T>()
            //    .Where(predicate)
            //    .Take(maxResults)
            //    .AsTableQuery<T>();

            //return await this.Query<T>(query);

            await Task.FromResult<bool>(false);

            throw new InvalidOperationException("Not Implemented");
        }

        /// <summary>
        /// Query
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
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
        #endregion

        #region Query Dictionary
        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<IDictionary<string, object>>> QueryByPartition(string partitionKey)
        {
            return await this.Query(new TableQuery().Where(TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, partitionKey)));
        }

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <remarks>
        /// Without providing the partion this query may not perform well.
        /// </remarks>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<IDictionary<string, object>>> QueryByRow(string rowKey)
        {
            return await this.Query(new TableQuery().Where(TableQuery.GenerateFilterCondition(TableStorage.RowKey, QueryComparisons.Equal, rowKey)));
        }

        /// <summary>
        /// Query By Partition and Row
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row</param>
        /// <returns></returns>
        public virtual async Task<IDictionary<string, object>> QueryByPartitionAndRow(string partitionKey, string rowKey)
        {
            var partitionFilter = TableQuery.GenerateFilterCondition(TableStorage.PartitionKey, QueryComparisons.Equal, partitionKey);
            var rowFilter = TableQuery.GenerateFilterCondition(TableStorage.RowKey, QueryComparisons.Equal, rowKey);
            var filter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, rowFilter);
            var query = new TableQuery().Where(filter);

            var result = await this.Query(query);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Generic Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Entities</returns>
        public virtual async Task<IEnumerable<IDictionary<string, object>>> Query(TableQuery query)
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

            var results = new List<IDictionary<string, object>>();
            foreach (var e in entities)
            {
                var dic = new Dictionary<string, object>();
                foreach (var p in e.Properties)
                {
                    dic.Add(p.Key, p.Value.PropertyAsObject);
                }
                dic.Add(TableStorage.PartitionKey, e.PartitionKey);
                dic.Add(TableStorage.RowKey, e.RowKey);
                dic.Add(TableStorage.ETag, e.ETag);
                dic.Add(TableStorage.Timestamp, e.Timestamp.DateTime);
                results.Add(dic);
            }

            return results;
        }
        #endregion

        #region Additional Methods
        /// <summary>
        /// Break Entities into batches
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Batches</returns>
        public virtual IEnumerable<IEnumerable<ITableEntity>> Batch(IEnumerable<ITableEntity> entities)
        {
            return entities.GroupBy(en => en.PartitionKey).SelectMany(e => this.Chunk<ITableEntity>(e));
        }

        /// <summary>
        /// Break Entities into batches
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Batches</returns>
        public virtual IEnumerable<IEnumerable<IDictionary<string, object>>> Batch(IEnumerable<IDictionary<string, object>> entities)
        {
            return entities.GroupBy(en => en[PartitionKey]).SelectMany(e => this.Chunk<IDictionary<string, object>>(e));
        }

        /// <summary>
        /// Chunk data into smaller blocks
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="entities">Entities</param>
        /// <returns>Chunks</returns>
        public virtual IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> entities)
        {
            return entities.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / TableStorage.MaimumxInsertBatch).Select(x => x.Select(v => v.Value));
        }
        #endregion
    }
}