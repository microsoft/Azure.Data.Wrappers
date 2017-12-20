namespace Azure.Data.Wrappers.Sanitization
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    public class SanitizedKeysTableEntity : TableEntity, ISupportsSanitizedKeys
    {
        #region Properties
        /// <summary>
        /// Input (Unsanitized) PartitionKey
        /// </summary>
        [IgnoreProperty]
        public string PartitionKeyUnsanitized { get; private set; }
        /// <summary>
        /// Input (Unsanitized) RowKey
        /// </summary>
        [IgnoreProperty]
        public string RowKeyUnsanitized { get; private set; }
        #endregion

        #region Constructors
        public SanitizedKeysTableEntity() : base()
        {
        }

        public SanitizedKeysTableEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
        #endregion

        #region Operations
        /// <summary>
        /// Sanitize the PartitionKey and RowKey.  Sets PartitionKeyUnsanitized and RowKeyUnsanitized to the original key values.
        /// </summary>
        /// <param name="sanitizationProvider">An ISanitizationProvider to provide sanitization logic</param>
        /// <returns>True if either key was changed.</returns>
        public bool SanitizeKeys(ISanitizationProvider sanitizationProvider)
        {
            if (sanitizationProvider == null) throw new ArgumentNullException(nameof(sanitizationProvider));

            this.PartitionKeyUnsanitized = this.PartitionKey;
            this.RowKeyUnsanitized = this.RowKey;

            this.PartitionKey = sanitizationProvider.Sanitize(this.PartitionKey);
            this.RowKey = sanitizationProvider.Sanitize(this.RowKey);

            return (this.PartitionKeyUnsanitized.Equals(this.PartitionKey) && this.RowKeyUnsanitized.Equals(this.RowKey)) ? false : true;
        }
        #endregion
    }
}
