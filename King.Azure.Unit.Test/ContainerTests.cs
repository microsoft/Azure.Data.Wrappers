namespace King.Azure.Unit.Test.Data
{
    using King.Azure.Data;
    using Microsoft.WindowsAzure.Storage;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class ContainerTests
    {
        private const string ConnectionString = "UseDevelopmentStorage=true;";

        [Test]
        public void Constructor()
        {
            new Container("test", ConnectionString);
        }

        [Test]
        public void IsIContainer()
        {
            Assert.IsNotNull(new Container("test", ConnectionString) as IContainer);
        }

        [Test]
        public void IsAzureStorage()
        {
            Assert.IsNotNull(new Container("test", ConnectionString) as AzureStorage);
        }

        [Test]
        public void ConstructorNameNull()
        {
            Assert.That(() => new Container(null, ConnectionString), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorAccountNameNull()
        {
            Assert.That(() => new Container(null, CloudStorageAccount.Parse(ConnectionString)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ConstructorKeyNull()
        {
            Assert.That(() => new Container("test", (string)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void DefaultCacheDuration()
        {
            Assert.AreEqual(31536000, Container.DefaultCacheDuration);
        }

        [Test]
        public void Name()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, ConnectionString);
            Assert.AreEqual(name, t.Name);
        }

        [Test]
        public void IsPublic()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, ConnectionString, true);
            Assert.IsTrue(t.IsPublic);
        }

        [Test]
        public void Client()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, ConnectionString);
            Assert.IsNotNull(t.Client);
        }

        [Test]
        public void Reference()
        {
            var name = Guid.NewGuid().ToString();
            var t = new Container(name, ConnectionString);
            Assert.IsNotNull(t.Reference);
        }

        [Test]
        public void DeleteBlobNameNull()
        {
            var c = new Container("test", ConnectionString);
            Assert.That(() => c.Delete(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ExistsBlobNameNull()
        {
            var c = new Container("test", ConnectionString);
            Assert.That(() => c.Exists(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GetBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Get<object>(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void StreamBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Stream(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SaveBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(null, new object()), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SaveObjectNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(Guid.NewGuid().ToString(), (object)null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetBytesBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Get(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GetTextBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.GetText(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SnapShotBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Snapshot(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SaveBytesBlobNameNull()
        {
            var random = new Random();
            var bytes = new byte[1024];
            random.NextBytes(bytes);

            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(null, bytes), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SaveTextBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(null, Guid.NewGuid().ToString()), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SaveBytesNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(Guid.NewGuid().ToString(), (byte[])null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SaveTextNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Save(Guid.NewGuid().ToString(), (string)null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GetBlockReferenceBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.GetBlockReference(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GetPageReferenceBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.GetPageReference(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void PropertiesBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Properties(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void SetCacheControlBlobNameNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.SetCacheControl(null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CopyFromToFromNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Copy(null, Guid.NewGuid().ToString()), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CopyFromToToNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Copy(Guid.NewGuid().ToString(), null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CopyFromNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Copy(null, c, Guid.NewGuid().ToString()), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CopyToNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Copy(Guid.NewGuid().ToString(), c, null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void CopyTargetNull()
        {
            var c = new Container("test", ConnectionString);

            Assert.That(() => c.Copy(Guid.NewGuid().ToString(), (IContainer)null, Guid.NewGuid().ToString()), Throws.TypeOf<ArgumentNullException>());
        }
    }
}