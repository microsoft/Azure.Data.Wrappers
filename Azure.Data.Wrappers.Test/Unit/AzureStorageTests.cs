namespace Azure.Data.Wrappers.Test.Unit
{
    using Azure.Data.Wrappers;
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
        public void ConstructorConnectionStringNull()
        {
            Assert.That(() => new AzureStorage(default(string)), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ConstructorAccountNull()
        {
            Assert.That(() => new AzureStorage(default(CloudStorageAccount)), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetSharedAccessSignatureThrowsOnNullPolicy()
        {
            var target = new AzureStorage(ConnectionString);

            Assert.That(() => target.GetSharedAccessSignature(default(SharedAccessAccountPolicy)), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetSharedAccessSignatureSuccess()
        {
            var target = new AzureStorage(ConnectionString);

            var result = target.GetSharedAccessSignature(new SharedAccessAccountPolicy());
        }
    }
}