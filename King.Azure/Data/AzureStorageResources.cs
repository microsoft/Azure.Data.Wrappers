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
            return from t in client.ListTables()
                   select t.Name;
        }

        /// <summary>
        /// List Container Names
        /// </summary>
        /// <returns>Container Names</returns>
        public virtual IEnumerable<string> ContainerNames()
        {
            var client = base.Account.CreateCloudBlobClient();
            return from t in client.ListContainers()
                   select t.Name;
        }

        /// <summary>
        /// List Queue Names
        /// </summary>
        /// <returns>Queue Names</returns>
        public virtual IEnumerable<string> QueueNames()
        {
            var client = base.Account.CreateCloudQueueClient();
            return from t in client.ListQueues()
                   select t.Name;
        }
        #endregion
    }
}