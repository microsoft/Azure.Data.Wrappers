namespace Azure.Data.Wrappers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

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

        /// <summary>
        /// Visibility Timeout
        /// </summary>
        protected readonly TimeSpan? visibilityTimeout = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connectionStringKey">Connection String</param>
        public StorageQueue(string name, string connectionString, TimeSpan? visibilityTimeout = null)
            : base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = base.Account.CreateCloudQueueClient();
            this.reference = client.GetQueueReference(name);
            this.visibilityTimeout = visibilityTimeout;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="account">Storage Account</param>
        public StorageQueue(string name, CloudStorageAccount account, TimeSpan? visibilityTimeout = null)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = base.Account.CreateCloudQueueClient();
            this.reference = client.GetQueueReference(name);
            this.visibilityTimeout = visibilityTimeout;
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
        /// <returns>Created</returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            return await this.reference.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Delete Queue
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Delete()
        {
            await this.reference.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get Cloud Queue Message
        /// </summary>
        /// <returns>Message</returns>
        public virtual async Task<CloudQueueMessage> Get()
        {
            return await this.reference.GetMessageAsync(this.visibilityTimeout, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Approixmate Message Count
        /// </summary>
        /// <returns>Message Count</returns>
        public virtual async Task<long?> ApproixmateMessageCount()
        {
            await this.reference.FetchAttributesAsync().ConfigureAwait(false);
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

            return await this.reference.GetMessagesAsync(messageCount, this.visibilityTimeout, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Cloud Queue Message
        /// </summary>
        /// <returns>Message</returns>
        public virtual async Task<T> GetAsync<T>()
        {
            var returned = await this.reference.GetMessageAsync(this.visibilityTimeout, null, null).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(returned.AsString);
        }

        /// <summary>
        /// Get Cloud Queue Message(s)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count">number of messages to get. Default is 5</param>
        /// <returns>Message</returns>
        public async Task<IEnumerable<T>> GetManyAsync<T>(int messageCount = 5)
        {
            if (0 >= messageCount)
            {
                messageCount = 1;
            }

            var returned = await this.reference.GetMessagesAsync(messageCount, this.visibilityTimeout, null, null).ConfigureAwait(false);
            return returned.Select(m => JsonConvert.DeserializeObject<T>(m.AsString));
        }

        /// <summary>
        /// Save Message to Queue
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public virtual async Task Send(CloudQueueMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await this.reference.AddMessageAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Save Model to queue, as json
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>Task</returns>
        public virtual async Task Send(object obj)
        {
            await this.SendAsync(obj).ConfigureAwait(false);
        }

        /// <summary>
        /// Save Model to queue, as json
        /// </summary>
        /// <param name="message">object</param>
        /// <returns>Task</returns>
        public virtual async Task SendAsync<T>(T message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message is CloudQueueMessage)
            {
                await this.Send(message as CloudQueueMessage).ConfigureAwait(false);
            }
            else
            {
                await this.Send(new CloudQueueMessage(JsonConvert.SerializeObject(message))).ConfigureAwait(false);
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

            await this.reference.DeleteMessageAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Clears the contents of the queue
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearAsync()
        {
            await this.reference.ClearAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Peeks the queue
        /// </summary>
        /// <param name="count">number of messages to peek with a default value of 1</param>
        /// <returns>Messages</returns>
        public virtual async Task<IEnumerable<CloudQueueMessage>> PeekAsync(int count = 1)
        {
            return await this.reference.PeekMessagesAsync(count).ConfigureAwait(false);
        }

        /// <summary>
        /// Peeks the queue
        /// </summary>
        /// <param name="count">number of messages to peek with a default value of 1</param>
        /// <returns>Messages</returns>
        public virtual async Task<IEnumerable<T>> PeekAsync<T>(int count = 1)
        {
            var returned = await this.reference.PeekMessagesAsync(count).ConfigureAwait(false);
            return returned.Select(m => JsonConvert.DeserializeObject<T>(m.AsString));
        }
        #endregion
    }
}