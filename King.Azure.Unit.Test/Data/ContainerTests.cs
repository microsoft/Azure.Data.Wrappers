namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class ContainerTests
    {
        private const string connectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new Container("test", connectionString);
        }

        [Test]
        public void IsIContainer()
        {
            Assert.IsNotNull(new Container("test", connectionString) as IContainer);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new Container(null, connectionString);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorKeyNull()
        {
            new Container("test", null);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, connectionString);
            Assert.AreEqual(name, t.Name);
        }

        [Test]
        public void IsPublic()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, connectionString, true);
            Assert.IsTrue(t.IsPublic);
        }

        [Test]
        public void Client()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, connectionString);
            Assert.IsNotNull(t.Client);
        }

        [Test]
        public void Reference()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, connectionString);
            Assert.IsNotNull(t.Reference);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Delete(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ExistsBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Exists(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Get<object>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SaveBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Save(null, new object());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveObjectNull()
        {
            var c = new Container("test", connectionString);
            await c.Save(Guid.NewGuid().ToString(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetBytesBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Get(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SaveBytesBlobNameNull()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var c = new Container("test", connectionString);
            await c.Save(null, bytes);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveBytesNull()
        {
            var c = new Container("test", connectionString);
            await c.Save(Guid.NewGuid().ToString(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetReferenceBlobNameNull()
        {
            var c = new Container("test", connectionString);
            c.GetReference(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task PropertiesBlobNameNull()
        {
            var c = new Container("test", connectionString);
            await c.Properties(null);
        }
    }
}