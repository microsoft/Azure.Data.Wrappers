namespace King.Azure.Unit.Test.Data
{
    using System;
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using NUnit.Framework;

    [TestFixture]
    public class FileShareTests
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=kingazure;AccountKey=LQFXI8kFSh0TR0dk2bvukQZRxymByGn1amCiR8chpIZ+NkLHqx6IFMcApHGWQutKpWfPloJfNv3ySM+uOJ3f9g==;";

        [Test]
        public void Constructor()
        {
            new FileShare("test", ConnectionString);
        }

        [Test]
        public void ConstructorAccount()
        {
            new FileShare("test", CloudStorageAccount.Parse(ConnectionString));
        }

        [Test]
        public void IsAzureStorage()
        {
            Assert.IsNotNull(new FileShare("test", ConnectionString) as AzureStorage);
        }

        [Test]
        public void IsIFileShare()
        {
            Assert.IsNotNull(new FileShare("test", ConnectionString) as IFileShare);
        }

        [Test]
        public void ConstructorNameNull()
        {
            Assert.That(() => new FileShare(null, ConnectionString), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorAccountNameNull()
        {
            Assert.That(() => new FileShare(null, CloudStorageAccount.Parse(ConnectionString)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Client()
        {
            var name = Guid.NewGuid().ToString();
            var t = new FileShare(name, ConnectionString);
            Assert.IsNotNull(t.Client);
        }

        [Test]
        public void Reference()
        {
            var name = Guid.NewGuid().ToString();
            var t = new FileShare(name, ConnectionString);
            Assert.IsNotNull(t.Reference);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new FileShare(name, ConnectionString);
            Assert.AreEqual(name, t.Name);
        }
    }
}