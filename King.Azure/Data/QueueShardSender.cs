namespace King.Azure.Data
{
    using System;
    using System.Threading.Tasks;

    public class QueueShardSender<T> : IQueueShardSender<T>
    {
        private readonly IStorageQueue[] queues;

        public IStorageQueue[] Queues
        {
            get
            {
                return this.queues;
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        public async Task Save(T obj, byte shardTarget = 0)
        {
            var random = new Random();
            var index = shardTarget == 0 ? random.Next(0, this.Queues.Length) : shardTarget;

            await this.queues[index].Save(obj);
        }
    }
}