namespace King.Azure.Data
{
    using System;
    using System.Collections.Generic;
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
        private readonly IStorageQueue[] queues;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="name">Name</param>
        /// <param name="shardCount">Shard Count</param>
        public StorageQueueShards(string connection, string name, byte shardCount = 0)
        {
            shardCount = shardCount > 0 ? shardCount : (byte)1;

            this.queues = new IStorageQueue[shardCount];
            for (var  i = 0; i < shardCount; i++)
            {
                var n = string.Format("{0}{1}", name, i);
                this.queues[i] = new StorageQueue(name, connection);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Queues
        /// </summary>
        public IReadOnlyCollection<IStorageQueue> Queues
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
        public async Task<bool> CreateIfNotExists()
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
        public async Task Delete()
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
        public async Task Save(object obj, byte shardTarget = 0)
        {
            var random = new Random();
            var index = shardTarget == 0 ? random.Next(0, this.queues.Count()) : shardTarget;

            await this.queues[index].Save(obj);
        }
        #endregion
    }
}