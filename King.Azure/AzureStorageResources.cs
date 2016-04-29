namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        public virtual async Task<IEnumerable<string>> TableNames()
        {
            TableContinuationToken token = null;
            var names = new List<string>();

            var client = base.Account.CreateCloudTableClient();

            do
            {
                var segments = await client.ListTablesSegmentedAsync(token);
                names.AddRange(segments.Results.Select(s => s.Name));
                token = segments.ContinuationToken;
            }
            while (null != token);

            return names;
        }

        /// <summary>
        /// List Tables
        /// </summary>
        /// <returns>Tables</returns>
        public virtual async Task<IEnumerable<ITableStorage>> Tables()
        {
            var tables = new List<ITableStorage>();

            var names = await this.TableNames();
            foreach (var name in names)
            {
                tables.Add(new TableStorage(name, base.Account));
            }

            return tables;
        }

        /// <summary>
        /// List Container Names
        /// </summary>
        /// <returns>Container Names</returns>
        public virtual async Task<IEnumerable<string>> ContainerNames()
        {
            BlobContinuationToken token = null;
            var names = new List<string>();

            var client = base.Account.CreateCloudBlobClient();

            do
            {
                var segments = await client.ListContainersSegmentedAsync(token);
                names.AddRange(segments.Results.Select(s => s.Name));
                token = segments.ContinuationToken;
            }
            while (null != token);

            return names;
        }

        /// <summary>
        /// List Containers
        /// </summary>
        /// <returns>Containers</returns>
        public virtual async Task<IEnumerable<IContainer>> Containers()
        {
            var containers = new List<IContainer>();
            var names = await this.ContainerNames();
            foreach (var name in names)
            {
                containers.Add(new Container(name, base.Account));
            }

            return containers;
        }

        /// <summary>
        /// List Queue Names
        /// </summary>
        /// <returns>Queue Names</returns>
        public virtual async Task<IEnumerable<string>> QueueNames()
        {
            QueueContinuationToken token = null;
            var names = new List<string>();

            var client = base.Account.CreateCloudQueueClient();

            do
            {
                var segments = await client.ListQueuesSegmentedAsync(token);
                names.AddRange(segments.Results.Select(s => s.Name));
                token = segments.ContinuationToken;
            }
            while (null != token);

            return names;
        }

        /// <summary>
        /// List Queues
        /// </summary>
        /// <returns>Queues</returns>
        public virtual async Task<IEnumerable<IStorageQueue>> Queues()
        {
            var queues = new List<IStorageQueue>();
            var names = await this.QueueNames();
            foreach (var name in names)
            {
                queues.Add(new StorageQueue(name, base.Account));
            }

            return queues;
        }
        #endregion
    }
}