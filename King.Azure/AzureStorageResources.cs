namespace King.Azure.Data
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Azure Storage Resources
    /// </summary>
    public class AzureStorageResources : AzureStorage, IAzureStorageResources
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Account</param>
        public AzureStorageResources(string connectionString)
            : base(connectionString)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// List Table Names
        /// </summary>
        /// <returns>Table Names</returns>
        public virtual IEnumerable<string> TableNames()
        {
            var client = base.Account.CreateCloudTableClient();
            return client.ListTables().Select(t => t.Name);
        }

        /// <summary>
        /// List Tables
        /// </summary>
        /// <returns>Tables</returns>
        public virtual IEnumerable<ITableStorage> Tables()
        {
            var names = this.TableNames();
            foreach (var name in names)
            {
                yield return new TableStorage(name, base.Account);
            }
        }

        /// <summary>
        /// List Container Names
        /// </summary>
        /// <returns>Container Names</returns>
        public virtual IEnumerable<string> ContainerNames()
        {
            var client = base.Account.CreateCloudBlobClient();
            return client.ListContainers().Select(c => c.Name);
        }

        /// <summary>
        /// List Containers
        /// </summary>
        /// <returns>Containers</returns>
        public virtual IEnumerable<IContainer> Containers()
        {
            var names = this.ContainerNames();
            foreach (var name in names)
            {
                yield return new Container(name, base.Account);
            }
        }

        /// <summary>
        /// List Queue Names
        /// </summary>
        /// <returns>Queue Names</returns>
        public virtual IEnumerable<string> QueueNames()
        {
            var client = base.Account.CreateCloudQueueClient();
            return client.ListQueues().Select(q => q.Name);
        }

        /// <summary>
        /// List Queues
        /// </summary>
        /// <returns>Queues</returns>
        public virtual IEnumerable<IStorageQueue> Queues()
        {
            var names = this.QueueNames();
            foreach (var name in names)
            {
                yield return new StorageQueue(name, base.Account);
            }
        }
        #endregion
    }
}