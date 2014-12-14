namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Storage Queue
    /// </summary>
    public class StorageQueue : AzureStorage, IStorageQueue
    {
        #region Members
        /// <summary>
        /// Cloud Queue Client
        /// </summary>
        private readonly CloudQueueClient client;

        /// <summary>
        /// Cloud Reference
        /// </summary>
        private readonly CloudQueue reference;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connectionStringKey">Connection String</param>
        public StorageQueue(string name, string connectionString)
            : base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = base.Account.CreateCloudQueueClient();
            this.reference = client.GetQueueReference(name);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="account">Storage Account</param>
        public StorageQueue(string name, CloudStorageAccount account)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = base.Account.CreateCloudQueueClient();
            this.reference = client.GetQueueReference(name);
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
        /// Cloud Queue Client
        /// </summary>
        public virtual CloudQueueClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// Cloud Reference
        /// </summary>
        public virtual CloudQueue Reference
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
        /// Delete Queue
        /// </summary>
        /// <returns></returns>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync();
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
        /// Approixmate Message Count
        /// </summary>
        /// <returns>Message Count</returns>
        public virtual async Task<long?> ApproixmateMessageCount()
        {
            await this.reference.FetchAttributesAsync();
            return this.reference.ApproximateMessageCount;
        }

        /// <summary>
        /// Get Many Cloud Queue Message
        /// </summary>
        /// <param name="messageCount">Message Count</param>
        /// <returns>Messages</returns>
        public virtual async Task<IEnumerable<CloudQueueMessage>> GetMany(int messageCount = 5)
        {
            if (0 >= messageCount)
            {
                messageCount = 1;
            }

            return await this.reference.GetMessagesAsync(messageCount);
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
        /// Save Model to queue, as json
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>Task</returns>
        public virtual async Task Save(object obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }

            if (obj is CloudQueueMessage)
            {
                await this.Save(obj as CloudQueueMessage);
            }
            else
            {
                await this.Save(new CloudQueueMessage(JsonConvert.SerializeObject(obj)));
            }
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