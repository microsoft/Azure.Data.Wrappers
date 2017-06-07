namespace King.Azure.Data
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.File;

    /// <summary>
    /// File Share
    /// </summary>
    public class FileShare : AzureStorage, IFileShare
    {
        #region Members
        /// <summary>
        /// Client
        /// </summary>
        private readonly CloudFileClient client;

        /// <summary>
        /// Reference
        /// </summary>
        private readonly CloudFileShare reference;
        #endregion

        #region Constructors
        /// <summary>
        /// File Share Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="connectionString">Connection String</param>
        public FileShare(string name, string connectionString)
            : base(connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = this.Account.CreateCloudFileClient();
            this.reference = this.client.GetShareReference(name);
        }

        /// <summary>
        /// File Share Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="account">Storage Account</param>
        public FileShare(string name, CloudStorageAccount account)
            : base(account)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name");
            }

            this.client = this.Account.CreateCloudFileClient();
            this.reference = this.client.GetShareReference(name);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.reference.Name;
            }
        }

        /// <summary>
        /// Client
        /// </summary>
        public virtual CloudFileClient Client
        {
            get
            {
                return this.client;
            }
        }

        /// <summary>
        /// Reference
        /// </summary>
        public virtual CloudFileShare Reference
        {
            get
            {
                return this.reference;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        /// <returns>Created</returns>
        public virtual async Task<bool> CreateIfNotExists()
        {
            return await this.reference.CreateIfNotExistsAsync();
        }
        #endregion
    }
}