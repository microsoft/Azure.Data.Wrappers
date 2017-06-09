namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;

    [TestFixture]
    public class AzureStorageResourcesTests
    {
        private readonly string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new AzureStorageResources(ConnectionString);
        }

        [Test]
        public void IsIAzureStorageResources()
        {
            Assert.IsNotNull(new AzureStorageResources(ConnectionString) as IAzureStorageResources);
        }

        [Test]
        public void IsAzureStorage()
        {
            Assert.IsNotNull(new AzureStorageResources(ConnectionString) as AzureStorage);
        }
    }
}