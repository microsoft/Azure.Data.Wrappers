namespace Azure.Data.Wrappers.Test.Unit
{
    using Azure.Data.Wrappers.Sanitization;
    using Azure.Data.Wrappers.Sanitization.Providers;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class SanitizedKeysTableEntityTests
    {
        [Test]
        public void ConstructorSetsKeys()
        {
            var target = new SanitizedKeysTableEntity("partitionKey", "rowKey");
            Assert.AreEqual(target.PartitionKey, "partitionKey");
            Assert.AreEqual(target.RowKey, "rowKey");
        }

        [Test]
        public void SanitizeKeysNullSanitizationProviderThrows()
        {
            var target = new SanitizedKeysTableEntity();
            Assert.That(() => target.SanitizeKeys(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SanitizeKeys_NothingToSanitize()
        {
            var target = new SanitizedKeysTableEntity();
            target.PartitionKey = "partitionKey";
            target.RowKey = "rowKey";

            var result = target.SanitizeKeys(new DefaultSanitizationProvider());

            Assert.IsFalse(result);
            Assert.AreEqual(target.PartitionKey, target.PartitionKeyUnsanitized);
            Assert.AreEqual(target.RowKey, target.RowKeyUnsanitized);
        }

        [Test]
        public void SanitizeKeys_SanitizePartitionKey()
        {
            var target = new SanitizedKeysTableEntity();
            target.PartitionKey = "partitionKey#";
            target.RowKey = "rowKey";

            var result = target.SanitizeKeys(new DefaultSanitizationProvider());

            Assert.IsTrue(result);
            Assert.AreEqual(target.PartitionKey, "partitionKey");
            Assert.AreEqual(target.PartitionKeyUnsanitized, "partitionKey#");
        }

        [Test]
        public void SanitizeKeys_SanitizeRowKey()
        {
            var target = new SanitizedKeysTableEntity();
            target.PartitionKey = "partitionKey";
            target.RowKey = "rowKey#";

            var result = target.SanitizeKeys(new DefaultSanitizationProvider());

            Assert.IsTrue(result);
            Assert.AreEqual(target.RowKey, "rowKey");
            Assert.AreEqual(target.RowKeyUnsanitized, "rowKey#");
        }
    }
}
