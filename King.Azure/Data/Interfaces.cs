namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        /// Delete Table
        /// </summary>
        /// <param name="tableName"></param>
        Task Delete();

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
        IEnumerable<T> QueryByRow<T>(string rowKey)
            where T : ITableEntity, new();
        T QueryByPartitionAndRow<T>(string partitionKey, string rowKey)
            where T : ITableEntity, new();
        #endregion
    }

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
        #endregion
    }

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

    /// <summary>
    /// IStorage Queue
    /// </summary>
    public interface IStorageQueue : IQueue<CloudQueueMessage>, IAzureStorage
    {
    }

    /// <summary>
    /// Azure Storage
    /// </summary>
    public interface IAzureStorage
    {
        #region Properties
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
        #endregion
    }

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
        #endregion
    }

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
}