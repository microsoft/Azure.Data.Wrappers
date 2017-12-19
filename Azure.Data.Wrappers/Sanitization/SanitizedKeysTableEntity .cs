namespace Azure.Data.Wrappers.Sanitization
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    public class SanitizedKeysTableEntity : TableEntity, ISupportsSanitizedKeys
    {
        public string PartitionKeyUnsanitized { get; private set; }
        public string RowKeyUnsanitized { get; private set; }

        public bool SanitizeKeys(ISanitizationProvider sanitizationProvider)
        {
            if (sanitizationProvider == null) throw new ArgumentNullException(nameof(sanitizationProvider));

            this.PartitionKey = sanitizationProvider.Sanitize(this.PartitionKey);
            this.RowKey = sanitizationProvider.Sanitize(this.RowKey);

            return (this.PartitionKeyUnsanitized.Equals(this.PartitionKey) && this.RowKeyUnsanitized.Equals(this.RowKey)) ? false : true;
        }
    }
}
