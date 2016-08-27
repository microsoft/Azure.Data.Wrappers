namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.File;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    #region IAccount
    /// <summary>
    /// Azure Storage Account
    /// </summary>
    public interface IStorageAccount
    {
        #region Properties
        /// <summary>
        /// Cloud Storage Account
        /// </summary>
        CloudStorageAccount Account
        {
            get;
        }
        #endregion
    }
    #endregion

    #region IStorageClient
    /// <summary>
    /// Storage Client Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStorageClient<T>
    {
        #region Properties
        /// <summary>
        /// Storage Client
        /// </summary>
        T Client
        {
            get;
        }
        #endregion
    }
    #endregion

    #region IStorageReference
    /// <summary>
    /// Storage Reference Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStorageReference<T>
    {
        #region Properties
        /// <summary>
        /// Storage Reference
        /// </summary>
        T Reference
        {
            get;
        }
        #endregion
    }
    #endregion

    #region ITableStorage
    /// <summary>
    /// Table Storage Interface
    /// </summary>
    public interface ITableStorage : IAzureStorage, IStorageReference<CloudTable>, IStorageClient<CloudTableClient>
    {
        #region Methods
        /// <summary>
        /// Create Table
        /// </summary>
        /// <param name="tableName">Table Name</param>
        Task<bool> Create();

        /// <summary>
        /// Insert or update the record in table
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TableResult> InsertOrReplace(ITableEntity entity);

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities"></param>
        Task<IEnumerable<TableResult>> Insert(IEnumerable<ITableEntity> entities);

        /// <summary>
        /// Insert Or Replace Entity (Dictionary)
        /// </summary>
        /// <remarks>
        /// Specify: PartitionKey, RowKey and ETag
        /// </remarks>
        /// <param name="entity">Entity</param>
        /// <returns>Result</returns>
        Task<TableResult> InsertOrReplace(IDictionary<string, object> entity);

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities">Entities</param>
        Task<IEnumerable<TableResult>> Insert(IEnumerable<IDictionary<string, object>> entities);

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partition"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryByPartition<T>(string partition)
            where T : ITableEntity, new();

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <remarks>
        /// Without providing the partion this query may not perform well.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryByRow<T>(string rowKey)
            where T : ITableEntity, new();

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        Task<T> QueryByPartitionAndRow<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new();

        /// <summary>
        /// Query
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">Table Query</param>
        /// <returns>Results</returns>
        Task<IEnumerable<T>> Query<T>(TableQuery<T> query)
            where T : ITableEntity, new();

        /// <summary>
        /// Query by Expression
        /// </summary>
        /// <typeparam name="T">Table Entity</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="maxResults">Max Result</param>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(Expression<Func<T, bool>> predicate, int maxResults = int.MaxValue)
            where T : ITableEntity, new();

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns>Entities</returns>
        Task<IEnumerable<IDictionary<string, object>>> QueryByPartition(string partitionKey);

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <remarks>
        /// Without providing the partion this query may not perform well.
        /// </remarks>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Entities</returns>
        Task<IEnumerable<IDictionary<string, object>>> QueryByRow(string rowKey);

        /// <summary>
        /// Query By Partition and Row
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row</param>
        /// <returns></returns>
        Task<IDictionary<string, object>> QueryByPartitionAndRow(string partitionKey, string rowKey);

        /// <summary>
        /// Generic Query
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Entities</returns>
        Task<IEnumerable<IDictionary<string, object>>> Query(TableQuery query);

        /// <summary>
        /// Delete By Partition
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <returns>Task</returns>
        Task DeleteByPartition(string partitionKey);

        /// <summary>
        /// Delete By Row
        /// </summary>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Task</returns>
        Task DeleteByRow(string rowKey);

        /// <summary>
        /// Delete By Partition and Row
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey"></param>
        /// <returns>Task</returns>
        Task DeleteByPartitionAndRow(string partitionKey, string row);

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        Task<TableResult> Delete(ITableEntity entity);

        /// <summary>
        /// Delete Entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns>Table Results</returns>
        Task<IEnumerable<TableResult>> Delete(IEnumerable<ITableEntity> entities);
        #endregion
    }
    #endregion

    #region IContainer
    /// <summary>
    /// Blob Container
    /// </summary>
    public interface IContainer : IAzureStorage, IStorageReference<CloudBlobContainer>, IStorageClient<CloudBlobClient>
    {
        #region Properties
        /// <summary>
        /// Is Public
        /// </summary>
        bool IsPublic
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Blob Exists
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>bool</returns>
        Task<bool> Exists(string blobName);

        /// <summary>
        /// Delete from Blob Storage
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="deleteHistory">Delete History (Snapshots)</param>
        /// <returns>Object</returns>
        Task Delete(string blobName, bool deleteHistory = true);

        /// <summary>
        /// Save Object as Json to Blob Storage
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="blobName">Blob Name</param>
        /// <param name="obj">Object</param>
        /// <returns>Task</returns>
        Task Save(string blobName, object obj);

        /// <summary>
        /// Get Object from Blob Storage
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Object</returns>
        Task<T> Get<T>(string blobName);

        /// <summary>
        /// Stream Blob
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Stream</returns>
        Task<Stream> Stream(string blobName);

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="snapshot">Snapshot time</param>
        /// <returns>Cloud Blob</returns>
        CloudBlockBlob GetBlockReference(string blobName, DateTimeOffset? snapshot = null);

        /// <summary>
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="snapshot">Snapshot time</param>
        /// <returns>Cloud Blob</returns>
        CloudPageBlob GetPageReference(string blobName, DateTimeOffset? snapshot = null);

        /// <summary>
        /// Save Binary Data
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="bytes">Bytes</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Task</returns>
        Task Save(string blobName, byte[] bytes, string contentType = "application/octet-stream");

        /// <summary>
        /// Save Text
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="text">Text</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Task</returns>
        Task Save(string blobName, string text, string contentType = "text/plain");

        /// <summary>
        /// Get Binary Data
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Bytes</returns>
        Task<byte[]> Get(string blobName);

        /// <summary>
        /// Get Bytes
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Text</returns>
        Task<string> GetText(string blobName);

        /// <summary>
        /// Blob Properties
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Blob Container Properties</returns>
        Task<BlobProperties> Properties(string blobName);

        /// <summary>
        /// Set Cache Control
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="cacheDuration">Cache Duration (Default 1 year)</param>
        /// <returns>Task</returns>
        Task SetCacheControl(string blobName, uint cacheDuration = Container.DefaultCacheDuration);

        /// <summary>
        /// List Blobs
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="useFlatBlobListing">Use Flat Blob Listing</param>
        /// <returns>Blobs</returns>
        Task<IEnumerable<IListBlobItem>> List(string prefix = null, bool useFlatBlobListing = false, BlobListingDetails details = BlobListingDetails.All, int? maxResults = int.MaxValue);

        /// <summary>
        /// Create Snapshot
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Task</returns>
        Task<ICloudBlob> Snapshot(string blobName);

        /// <summary>
        /// Copy from, to seperate container/blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="target">Target</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        Task<string> Copy(string from, IContainer target, string to);

        /// <summary>
        /// Copy from, to seperate container/blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="target">Target</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        Task<string> Copy(string from, string target, string to);

        /// <summary>
        /// Copy From Blob to Blob
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <returns>Blob Uri</returns>
        Task<string> Copy(string from, string to);
        #endregion
    }
    #endregion

    #region IQueueObject
    /// <summary>
    /// Queue Object Interface
    /// </summary>
    public interface IQueueObject
    {
        #region Methods
        /// <summary>
        /// Save Specific Message to Queue
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Task</returns>
        Task Send(object obj);
        #endregion
    }
    #endregion

    #region IQueue<T>
    /// <summary>
    /// IQueue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue<T>
    {
        #region Methods
        /// <summary>
        /// Get Cloud Queue Message
        /// </summary>
        /// <returns>Message</returns>
        Task<T> Get();

        /// <summary>
        /// Delete Message from Queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task Delete(T message);

        /// <summary>
        /// Save Message to Queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        Task Send(T message);
        #endregion
    }
    #endregion

    #region IStorageQueue
    /// <summary>
    /// Storage Queue Interface
    /// </summary>
    public interface IStorageQueue : IQueue<CloudQueueMessage>, IQueueObject, IAzureStorage, IStorageReference<CloudQueue>, IStorageClient<CloudQueueClient>, IQueueCount
    {
        #region Methods
        /// <summary>
        /// Get Many Cloud Queue Message
        /// </summary>
        /// <returns>Messages</returns>
        Task<IEnumerable<CloudQueueMessage>> GetMany(int messageCount = 5);
        #endregion
    }
    #endregion

    #region IQueueCount
    /// <summary>
    /// Queue Count
    /// </summary>
    public interface IQueueCount
    {
        #region Methods
        /// <summary>
        /// Approixmate Message Count
        /// </summary>
        /// <returns>Message Count</returns>
        Task<long?> ApproixmateMessageCount();
        #endregion
    }
    #endregion

    #region IAzureStorage
    /// <summary>
    /// Azure Storage
    /// </summary>
    public interface IAzureStorage
    {
        #region Properties
        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIfNotExists();

        /// <summary>
        /// Delete Item
        /// </summary>
        /// <returns>Task</returns>
        Task Delete();
        #endregion
    }
    #endregion

    #region IProcessor
    /// <summary>
    /// IProcessor
    /// </summary>
    public interface IProcessor<T>
    {
        #region Methods
        /// <summary>
        /// Process Data
        /// </summary>
        /// <param name="data">Data to Process</param>
        /// <returns>Successful</returns>
        Task<bool> Process(T data);
        #endregion
    }
    #endregion

    #region IPoller
    /// <summary>
    /// Store Poller Interface
    /// </summary>
    /// <typeparam name="T">Dequeue Type</typeparam>
    public interface IPoller<T>
    {
        #region Methods
        /// <summary>
        /// Poll for Queued Message
        /// </summary>
        /// <returns>Queued Item</returns>
        Task<IQueued<T>> Poll();

        /// <summary>
        /// Poll for Queued Message
        /// </summary>
        /// <returns>Queued Item</returns>
        Task<IEnumerable<IQueued<T>>> PollMany(int messageCount = 5);
        #endregion
    }
    #endregion

    #region IStorageQueuePoller
    /// <summary>
    /// Storage Queue Poller Interface
    /// </summary>
    /// <typeparam name="T">Dequeue Type</typeparam>
    public interface IStorageQueuePoller<T> : IPoller<T>
    {
        #region Properties
        /// <summary>
        /// Storage Queue
        /// </summary>
        IStorageQueue Queue
        {
            get;
        }
        #endregion
    }
    #endregion

    #region IQueued
    /// <summary>
    /// IQueued
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueued<T>
    {
        #region Methods
        /// <summary>
        /// Delete Message
        /// </summary>
        /// <returns>Task</returns>
        Task Complete();

        /// <summary>
        /// Abandon Message
        /// </summary>
        /// <returns>Task</returns>
        Task Abandon();

        /// <summary>
        /// Data
        /// </summary>
        /// <returns>Data</returns>
        Task<T> Data();
        #endregion
    }
    #endregion

    #region IAzureStorageResources
    /// <summary>
    /// Azure Storage Resources Interface
    /// </summary>
    public interface IAzureStorageResources
    {
        #region Methods
        /// <summary>
        /// List Table Names
        /// </summary>
        /// <returns>Table Names</returns>
        Task<IEnumerable<string>> TableNames();

        /// <summary>
        /// List Tables
        /// </summary>
        /// <returns>Tables</returns>
        Task<IEnumerable<ITableStorage>> Tables();

        /// <summary>
        /// List Container Names
        /// </summary>
        /// <returns>Container Names</returns>
        Task<IEnumerable<string>> ContainerNames();

        /// <summary>
        /// List Containers
        /// </summary>
        /// <returns>Containers</returns>
        Task<IEnumerable<IContainer>> Containers();

        /// <summary>
        /// List Queue Names
        /// </summary>
        /// <returns>Queue Names</returns>
        Task<IEnumerable<string>> QueueNames();

        /// <summary>
        /// List Queues
        /// </summary>
        /// <returns>Queues</returns>
        Task<IEnumerable<IStorageQueue>> Queues();
        #endregion
    }
    #endregion

    #region IFileShare
    /// <summary>
    /// File Share Interface
    /// </summary>
    public interface IFileShare : IStorageReference<CloudFileShare>, IStorageClient<CloudFileClient>
    {
    }
    #endregion

    #region QueueShardSender
    /// <summary>
    /// Queue Shard Sender
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueueShardSender<T>
    {
        #region Properties
        /// <summary>
        /// Queues
        /// </summary>
        IReadOnlyCollection<T> Queues
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Queue Message to shard, 0 means at random
        /// </summary>
        /// <param name="obj">message</param>
        /// <param name="shardTarget">Shard Target</param>
        /// <returns>Task</returns>
        Task Save(object obj, byte shardTarget = 0);

        /// <summary>
        /// Create all queues
        /// </summary>
        /// <returns></returns>
        Task<bool> CreateIfNotExists();

        /// <summary>
        /// Delete all queues
        /// </summary>
        /// <returns>Task</returns>
        Task Delete();
        #endregion
    }
    #endregion
}