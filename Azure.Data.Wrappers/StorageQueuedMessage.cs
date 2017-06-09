namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// Queued Message
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class StorageQueuedMessage<T> : IQueued<T>
    {
        #region Members
        /// <summary>
        /// Storage Queue
        /// </summary>
        protected readonly IStorageQueue queue = null;

        /// <summary>
        /// Cloud Queue Message
        /// </summary>
        protected readonly CloudQueueMessage message = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="queue">Queue</param>
        /// <param name="message">Cloud Queue Message</param>
        public StorageQueuedMessage(IStorageQueue queue, CloudQueueMessage message)
        {
            if (null == queue)
            {
                throw new ArgumentNullException("queue");
            }
            if (null == message)
            {
                throw new ArgumentNullException("message");
            }

            this.queue = queue;
            this.message = message;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Delete Message
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Complete()
        {
            await this.queue.Delete(this.message);
        }

        /// <summary>
        /// Abandon Message
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Abandon()
        {
            await Task.Factory.StartNew(() => { }); //No Abandon?
        }

        /// <summary>
        /// Data
        /// </summary>
        /// <returns>Data</returns>
        public virtual async Task<T> Data()
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(this.message.AsString));
        }
        #endregion
    }
}