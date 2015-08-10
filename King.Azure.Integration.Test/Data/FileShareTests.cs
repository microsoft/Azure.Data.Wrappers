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
            var random = new Random();
            var storage = new FileShare(string.Format("a{0}b", random.Next()), ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }
    }
}