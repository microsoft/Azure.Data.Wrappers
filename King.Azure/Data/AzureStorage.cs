namespace King.Azure.Data
{
    using System;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// Azure Storage
    /// </summary>
    public class AzureStorage : IStorageAccount
    {
        #region Members
        /// <summary>
        /// Cloud Storage Account
        /// </summary>
        private readonly CloudStorageAccount account;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Account</param>
        public AzureStorage(string connectionString)
            : this(CloudStorageAccount.Parse(connectionString))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="account">Storage Account</param>
        public AzureStorage(CloudStorageAccount account)
        {
            if (null == account)
            {
                throw new ArgumentNullException("account");
            }

            this.account = account;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Cloud Storage Account
        /// </summary>
        public CloudStorageAccount Account
        {
            get
            {
                return this.account;
            }
        }
        #endregion
    }
}