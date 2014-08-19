namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Constructor()
        {
            new Container("test", "UseDevelopmentStorage=true");
        }

        [Test]
        public void IsIContainer()
        {
            Assert.IsNotNull(new Container("test", "UseDevelopmentStorage=true") as IContainer);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorTableNull()
        {
            new Container(null, "UseDevelopmentStorage=true");
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
            var t = new Container(name, "UseDevelopmentStorage=true");
            Assert.AreEqual(name, t.Name);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteBlobNameNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Delete(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetBlobNameNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Get<object>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SaveBlobNameNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Save(null, new object());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveObjectNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Save(Guid.NewGuid().ToString(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetBytesBlobNameNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Get(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SaveBytesBlobNameNull()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Save(null, bytes);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveBytesNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            await c.Save(Guid.NewGuid().ToString(), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetReferenceBlobNameNull()
        {
            var c = new Container("test", "UseDevelopmentStorage=true");
            c.GetReference(null);
        }
    }
}