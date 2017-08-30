using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Azure.Data.Wrappers
{
    public class AzureStorageFactory : IAzureStorageFactory
    {
        public IStorageAccount GetAccount(string accountName, string key, bool useHttps)
        {
            if (string.IsNullOrEmpty(accountName)) throw new ArgumentNullException(nameof(accountName));
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            return new AzureStorage(new CloudStorageAccount(new StorageCredentials(accountName, key), useHttps));
        }

        public IStorageAccount GetAccount(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            return new AzureStorage(connectionString);
        }

        public IStorageQueue GetAzureQueue<T>(IStorageAccount storageAccount, string queueName, int visibilityTimeoutInMS = 300000)
        {
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));
            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException(nameof(queueName));

            return new StorageQueue(queueName, storageAccount.Account, new TimeSpan(0, 0, 0, 0, visibilityTimeoutInMS));
        }

        public ITableStorage GetAzureTable(IStorageAccount storageAccount, string tableName)
        {
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));

            return new TableStorage(tableName, storageAccount.Account);
        }

        public IContainer GetBlobFileContainer(IStorageAccount storageAccount, string containerName, bool isPublic = false, LocationMode location = LocationMode.PrimaryThenSecondary)
        {
            if (storageAccount == null) throw new ArgumentNullException(nameof(storageAccount));
            if (string.IsNullOrEmpty(containerName)) throw new ArgumentNullException(nameof(containerName));

            return new Container(containerName, storageAccount.Account, isPublic, location);
        }
    }
}
