namespace King.Service.Integration
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class ContainerTests
    {
        private readonly string ConnectionString = "UseDevelopmentStorage=true;";

        #region Helper
        private class Helper
        {
            public Guid Id
            {
                get;
                set;
            }
        }
        #endregion

        [Test]
        public async Task CreateIfNotExists()
        {
            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            Assert.IsTrue(created);
        }

        [Test]
        public async Task RoundTrip()
        {
            var helper = new Helper()
            {
                Id = Guid.NewGuid(),
            };

            var name = 'a' + Guid.NewGuid().ToString().ToLowerInvariant().Replace('-', 'a');
            var blobName = "happyBlob";
            var storage = new Container(name, ConnectionString);
            var created = await storage.CreateIfNotExists();

            await storage.Save(blobName, helper);
            var returned = await storage.Get<Helper>(blobName);
            Assert.IsNotNull(returned);
            Assert.AreEqual(helper.Id, returned.Id);
        }
    }
}