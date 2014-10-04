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
        /// List Tables
        /// </summary>
        /// <returns>Table Names</returns>
        public virtual IEnumerable<string> Tables()
        {
            var client = base.Account.CreateCloudTableClient();
            return from t in client.ListTables()
                   select t.Name;
        }

        /// <summary>
        /// List Containers
        /// </summary>
        /// <returns>Containers</returns>
        public virtual IEnumerable<string> Containers()
        {
            var client = base.Account.CreateCloudBlobClient();
            return from t in client.ListContainers()
                   select t.Name;
        }

        /// <summary>
        /// List Queues
        /// </summary>
        /// <returns>Queue Names</returns>
        public virtual IEnumerable<string> Queues()
        {
            var client = base.Account.CreateCloudQueueClient();
            return from t in client.ListQueues()
                   select t.Name;
        }
        #endregion
    }
}