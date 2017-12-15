namespace Azure.Data.Wrappers.Test.Integration
{
    using Azure.Data.Wrappers;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    [Category("Integration")]
    public class FileShareTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        [Ignore("not supported by development storage")]
        public async Task CreateIfNotExists()
        {
            var random = new Random();
            var storage = new FileShare(string.Format("a{0}b", random.Next()), ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }
    }
}