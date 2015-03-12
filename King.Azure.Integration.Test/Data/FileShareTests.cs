namespace King.Azure.Integration.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class FileShareTests
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=kingazure;AccountKey=LQFXI8kFSh0TR0dk2bvukQZRxymByGn1amCiR8chpIZ+NkLHqx6IFMcApHGWQutKpWfPloJfNv3ySM+uOJ3f9g==;";

        [Test]
        public async Task CreateIfNotExists()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new FileShare(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }
    }
}