namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class AzureStorageResourcesTests
    {
        [Test]
        public void Constructor()
        {
            new AzureStorageResources(Guid.NewGuid().ToString());
        }

        [Test]
        public void IsIAzureStorageResources()
        {
            Assert.IsNotNull(new AzureStorageResources(Guid.NewGuid().ToString()) as IAzureStorageResources);
        }

        [Test]
        public void IsAzureStorage()
        {
            Assert.IsNotNull(new AzureStorageResources(Guid.NewGuid().ToString()) as AzureStorage);
        }
    }
}