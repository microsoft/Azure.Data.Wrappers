namespace King.Azure.Data
{
    using Microsoft.WindowsAzure.Storage;
    using System;

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
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            this.account = CloudStorageAccount.Parse(connectionString);
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