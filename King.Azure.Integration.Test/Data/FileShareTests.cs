namespace King.Azure.Integration.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class FileShareTests
    {

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