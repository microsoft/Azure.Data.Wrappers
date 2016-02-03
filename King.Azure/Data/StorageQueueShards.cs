namespace King.Azure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Queue;

    public class QueueShardManager
    {
        public readonly IQueue<object>[] Queues;

        public QueueShardManager(string connection, string name, byte shardCount = 0)
        {
            shardCount = shardCount > 0 ? shardCount : (byte)1;

            this.Queues = new IQueue<object>[shardCount];
        }

        public async Task Save(object obj, byte shardTarget = 0)
        {
            var random = new Random();
            var index = shardTarget == 0 ? random.Next(0, this.Queues.Length) : shardTarget;

            await this.Queues[index].Save(obj);
        }
    }
}