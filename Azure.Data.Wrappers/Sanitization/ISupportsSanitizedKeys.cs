namespace Azure.Data.Wrappers
{
    using Azure.Data.Wrappers.Sanitization;
    using Microsoft.WindowsAzure.Storage.Table;

    public interface ISupportsSanitizedKeys : ITableEntity
    {
        [IgnoreProperty]
        string PartitionKeyUnsanitized { get; }
        [IgnoreProperty]
        string RowKeyUnsanitized { get; }
        bool SanitizeKeys(ISanitizationProvider sanitizationProvider);
    }
}