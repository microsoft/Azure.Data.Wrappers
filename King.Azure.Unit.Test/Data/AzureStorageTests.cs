namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class AzureStorageTests
    {
        const string ConnectionString = "UseDevelopmentStorage=true";

        [Test]
        public void Constructor()
        {
            new AzureStorage(ConnectionString);
        }

        [Test]
        public void IsIStorageAccount()
        {
            Assert.IsNotNull(new AzureStorage(ConnectionString) as IStorageAccount);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorConnectionStringNull()
        {
            new AzureStorage((string)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorAccountNull()
        {
            new AzureStorage((CloudStorageAccount)null);
        }
    }
}