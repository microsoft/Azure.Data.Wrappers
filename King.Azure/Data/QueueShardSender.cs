namespace King.Azure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>
    /// Queue Shard Sender
    /// </summary>
    public class QueueShardSender : IQueueShardSender<IStorageQueue>
    {
        #region Members
        /// <summary>
        /// Queues
        /// </summary>
        private readonly IStorageQueue[] queues;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <param name="shardCount"></param>
        public QueueShardSender(string connection, string name, byte shardCount = 0)
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
        public async Task<bool> CreateIfNotExists()
        {
            var success = true;
            foreach (var q in this.queues)
            {
                success &= await q.CreateIfNotExists();
            }
            return success;
        }

        public async Task Delete()
        {
            foreach (var q in this.queues)
            {
                await q.Delete();
            }
        }

        public async Task Save(object obj, byte shardTarget = 0)
        {
            var random = new Random();
            var index = shardTarget == 0 ? random.Next(0, this.queues.Count()) : shardTarget;

            await this.queues[index].Save(obj);
        }
        #endregion
    }
}