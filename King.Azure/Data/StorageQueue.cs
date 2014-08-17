namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Queue
    /// </summary>
    public class StorageQueue : AzureStorage, IStorageQueue
    {
        #region Members
        /// <summary>
        /// Client
        /// </summary>
        protected readonly CloudQueueClient client;

        /// <summary>
        /// Reference
        /// </summary>
        protected readonly CloudQueue reference;
        #endregion

        #region Constructors
        /// <summary>
        /// Queue
        /// </summary>
        /// <param name="name">Name</param>
        public StorageQueue(string name, string connectionString)
            : base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = base.account.CreateCloudQueueClient();
            this.reference = client.GetQueueReference(name);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Table Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.reference.Name;
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
        /// Get Cloud Queue Message
        /// </summary>
        /// <returns>Message</returns>
        public virtual async Task<CloudQueueMessage> Get()
        {
            return await this.reference.GetMessageAsync();
        }

        /// <summary>
        /// Save Message to Queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public virtual async Task Save(CloudQueueMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            await this.reference.AddMessageAsync(message);
        }

        /// <summary>
        /// Delete Message from Queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public virtual async Task Delete(CloudQueueMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            await this.reference.DeleteMessageAsync(message);
        }
        #endregion
    }
}
