namespace King.Azure.Integration.Test.Data
{
    using System;
    using System.Threading.Tasks;
    using King.Azure.Data;
    using NUnit.Framework;

    [TestFixture]
    public class FileShareTests
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=kingazure;AccountKey=nCJim8J7DwyAsk1ULffZLCjKZ/iVvULDG3dlbTi8XoXUcztB1jQss3VbEHdVIUlkl3866JA0hg/01BTkFKklWQ==";

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