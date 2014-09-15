namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
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

    #region ITableStorage
    /// <summary>
    /// Table Storage Interface
    /// </summary>
    public interface ITableStorage : IAzureStorage
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
        /// <param name="item">Scheduled Task Entry</param>
        Task<TableResult> InsertOrReplace(ITableEntity entry);

        /// <summary>
        /// Insert Batch
        /// </summary>
        /// <param name="entities"></param>
        Task<IEnumerable<TableResult>> Insert(IEnumerable<ITableEntity> entities);

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partition"></param>
        /// <returns></returns>
        IEnumerable<T> QueryByPartition<T>(string partition)
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
        IEnumerable<T> QueryByRow<T>(string rowKey)
            where T : ITableEntity, new();

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        T QueryByPartitionAndRow<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task DeleteByPartition(string partitionKey);

        /// <summary>
        /// Delete By Partition and Row
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey"></param>
        /// <returns>Task</returns>
        Task DeleteByPartitionAndRow(string partitionKey, string row);
        #endregion

        #region Properties
        /// <summary>
        /// Table Client
        /// </summary>
        CloudTableClient Client
        {
            get;
        }

        /// <summary>
        /// Table
        /// </summary>
        CloudTable Reference
        {
            get;
        }
        #endregion
    }
    #endregion

    #region IContainer
    /// <summary>
    /// Blob Container
    /// </summary>
    public interface IContainer : IAzureStorage
    {
        #region Methods
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
        /// Get Reference
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Cloud Blob</returns>
        CloudBlockBlob GetReference(string blobName);

        /// <summary>
        /// Save Binary Data
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <param name="bytes">Bytes</param>
        /// <param name="contentType">Content Type</param>
        /// <returns>Task</returns>
        Task Save(string blobName, byte[] bytes, string contentType = "application/octet-stream");
        
        /// <summary>
        /// Get Binary Data
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Bytes</returns>
        Task<byte[]> Get(string blobName);

        /// <summary>
        /// Blob Properties
        /// </summary>
        /// <param name="blobName">Blob Name</param>
        /// <returns>Blob Container Properties</returns>
        Task<BlobProperties> Properties(string blobName);

        /// <summary>
        /// List Blobs
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="useFlatBlobListing">Use Flat Blob Listing</param>
        /// <returns>Blobs</returns>
        IEnumerable<IListBlobItem> List(string prefix = null, bool useFlatBlobListing = false);
        #endregion

        #region Properties
        /// <summary>
        /// Is Public
        /// </summary>
        bool IsPublic
        {
            get;
        }

        /// <summary>
        /// Client
        /// </summary>
        CloudBlobClient Client
        {
            get;
        }

        /// <summary>
        /// Reference
        /// </summary>
        CloudBlobContainer Reference
        {
            get;
        }
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
        Task Save(T message);
        #endregion
    }
    #endregion

    #region IStorageQueue
    /// <summary>
    /// IStorage Queue
    /// </summary>
    public interface IStorageQueue : IQueue<CloudQueueMessage>, IAzureStorage
    {
        #region Methods
        /// <summary>
        /// Get Many Cloud Queue Message
        /// </summary>
        /// <returns>Messages</returns>
        Task<IEnumerable<CloudQueueMessage>> GetMany(int messageCount = 5);
        #endregion

        #region Properties
        /// <summary>
        /// Cloud Queue Client
        /// </summary>
        CloudQueueClient Client
        {
            get;
        }

        /// <summary>
        /// Cloud Reference
        /// </summary>
        CloudQueue Reference
        {
            get;
        }
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
    /// IPoller
    /// </summary>
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
}