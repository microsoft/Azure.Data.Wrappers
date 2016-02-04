namespace King.Azure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Queue Shard Sender
    /// </summary>
    public class StorageQueueShards : IQueueShardSender<IStorageQueue>
    {
        #region Members
        /// <summary>
        /// Queues
        /// </summary>
        protected readonly IReadOnlyCollection<IStorageQueue> queues;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connection">Connection</param>
        /// <param name="shardCount">Shard Count</param>
        public StorageQueueShards(string name, string connection, byte shardCount = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentException("connection");
            }

            shardCount = shardCount > 0 ? shardCount : (byte)2;

            var qs = new IStorageQueue[shardCount];
            for (var i = 0; i < shardCount; i++)
            {
                var n = string.Format("{0}{1}", name, i);
                qs[i] = new StorageQueue(n, connection);
            }

            this.queues = new ReadOnlyCollection<IStorageQueue>(qs.ToList());
        }

        /// <summary>
        /// Constructor for mocking
        /// </summary>
        /// <param name="queues">Queues</param>
        public StorageQueueShards(IEnumerable<IStorageQueue> queues)
        {
            if (null == queues)
            {
                throw new ArgumentNullException("queue");
            }

            if (0 == queues.Count())
            {
                throw new ArgumentException("Queues length is 0.");
            }

            this.queues = new ReadOnlyCollection<IStorageQueue>(queues.ToList());
        }
        #endregion

        #region Properties
        /// <summary>
        /// Queues
        /// </summary>
        public virtual IReadOnlyCollection<IStorageQueue> Queues
        {
            get
            {
                return this.queues;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Queue Message
        /// </summary>
        /// <param name="obj">message</param>
        /// <param name="shardTarget">Shard Target</param>
        /// <returns>Task</returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            var success = true;
            foreach (var q in this.queues)
            {
                success &= await q.CreateIfNotExists();
            }
            return success;
        }

        /// <summary>
        /// Delete all queues
        /// </summary>
        /// <returns>Task</returns>
        public virtual async Task Delete()
        {
            foreach (var q in this.queues)
            {
                await q.Delete();
            }
        }

        /// <summary>
        /// Queue Message to shard, 0 means at random
        /// </summary>
        /// <param name="obj">message</param>
        /// <param name="shardTarget">Shard Target</param>
        /// <returns>Task</returns>
        public virtual async Task Save(object obj, byte shardTarget = 0)
        {
            var random = new Random();
            var index = shardTarget == 0 ? random.Next(0, this.queues.Count()) : shardTarget;

            var q = this.queues.ElementAt(index);
            await q.Save(obj);
        }
        #endregion
    }
}