namespace King.Azure.Data
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Queue Poller
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class StorageQueuePoller<T> : IPoller<T>
    {
        #region Members
        /// <summary>
        /// Queue
        /// </summary>
        protected readonly IStorageQueue queue = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <param name="connectionString">Connection String</param>
        public StorageQueuePoller(string queueName, string connectionString)
            : this(new StorageQueue(queueName, connectionString))
        {
        }

        /// <summary>
        /// Constructor for Mocking
        /// </summary>
        /// <param name="queue">Queue</param>
        public StorageQueuePoller(IStorageQueue queue)
        {
            if (null == queue)
            {
                throw new ArgumentNullException("queue");
            }

            this.queue = queue;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Poll for Queued Message
        /// </summary>
        /// <returns>Queued Item</returns>
        public virtual async Task<IQueued<T>> Poll()
        {
            var msg = await this.queue.Get();
            return null == msg ? null : new StorageQueuedMessage<T>(this.queue, msg);
        }
        #endregion
    }
}