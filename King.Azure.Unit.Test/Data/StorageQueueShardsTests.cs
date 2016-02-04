namespace King.Azure.Unit.Test.Data
{
    using System;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;

    [TestFixture]
    public class StorageQueueShardsTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new StorageQueueShards("test", ConnectionString, 2);
        }
    }
}