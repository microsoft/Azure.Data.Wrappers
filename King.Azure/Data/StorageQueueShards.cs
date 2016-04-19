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
    public class StorageQueueShards : IQueueShardSender<IStorageQueue>, IAzureStorage
    {
        #region Members
        /// <summary>
        /// Queues
        /// </summary>
        protected readonly IEnumerable<IStorageQueue> queues;

        /// <summary>
        /// Base of the Name
        /// </summary>
        protected readonly string baseName;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connection">Connection</param>
        /// <param name="shardCount">Shard Count</param>
        public StorageQueueShards(string name, string connection, byte shardCount = 2)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentException("connection");
            }

            this.baseName = name;
            shardCount = shardCount > 0 ? shardCount : (byte)2;

            var qs = new IStorageQueue[shardCount];
            for (var i = 0; i < shardCount; i++)
            {
                var n = string.Format("{0}{1}", this.baseName, i);
                qs[i] = new StorageQueue(n, connection);
            }

            this.queues = new ReadOnlyCollection<IStorageQueue>(qs);
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

            this.queues = queues;
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
                return new ReadOnlyCollection<IStorageQueue>(this.queues.ToList());
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.baseName;
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
            var index = this.Index(shardTarget);
            var q = this.queues.ElementAt(index);
            await q.Send(obj);
        }

        /// <summary>
        /// Determine index of queues to interact with
        /// </summary>
        /// <remarks>
        /// Specifically broken out for testing safety
        /// </remarks>
        /// <param name="shardTarget">Shard Target</param>
        /// <returns>Index</returns>
        public virtual byte Index(byte shardTarget)
        {
            var random = new Random();
            var count = this.queues.Count();
            return shardTarget == 0 || shardTarget > count ? (byte)random.Next(0, count) : shardTarget;
        }
        #endregion
    }
}