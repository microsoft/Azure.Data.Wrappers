namespace King.Azure.Data
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
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

        /// <summary>
        /// Poll for Queued Message
        /// </summary>
        /// <returns>Queued Item</returns>
        public virtual async Task<IEnumerable<IQueued<T>>> PollMany(int messageCount = 5)
        {
            if (0 > messageCount)
            {
                throw new ArgumentException("Message count must be greater than 0.");
            }

            var msgs = await this.queue.GetMany(messageCount);
            if (null == msgs)
            {
                return null;
            }

            return from m in msgs
                   where msgs != null
                   select new StorageQueuedMessage<T>(this.queue, m);
        }
        #endregion
    }
}