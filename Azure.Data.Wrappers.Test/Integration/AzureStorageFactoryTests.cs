using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using NSubstitute;
using NUnit.Framework;

namespace Azure.Data.Wrappers.Test.Integration
{
    [SetUpFixture]
    [TestFixture]
    [Category("Integration")]
    public class AzureStorageFactoryTests
    {
        private static IStorageAccount mockAccount;

        [OneTimeSetUp]
        public void ClassInit()
        {
            mockAccount = Substitute.For<IStorageAccount>();
            mockAccount.Account.Returns(new CloudStorageAccount(new StorageCredentials(TestHelpers.DevAccountName, TestHelpers.DevAccountKey), true));
        }

        [TestFixture]
        [Category("Integration")]
        public class CreateAccount
        {
            [Test]
            public void ValidatesArgs()
            {
                //Arrange
                var target = new AzureStorageFactory();

                //Act Assert
                Assert.That(() => target.GetAccount(null), Throws.ArgumentNullException);
                Assert.That(() => target.GetAccount(null, "key", true), Throws.ArgumentNullException);
                Assert.That(() => target.GetAccount("foo", null, true), Throws.ArgumentNullException);
            }

            [Test]
            public void SuccessWithConnectionString()
            {
                //Arrange
                var target = new AzureStorageFactory();

                //Act
                var result = target.GetAccount(TestHelpers.DevConnectionString);

                //Assert
                Assert.IsNotNull(result);
            }

            [Test]
            public void SuccessWithAccountName()
            {
                //Arrange
                var target = new AzureStorageFactory();

                //Act
                var result = target.GetAccount(TestHelpers.DevAccountName, TestHelpers.DevAccountKey, true);

                //Assert
                Assert.IsNotNull(result);
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class GetAzureQueue
        {
            [Test]
            public void ValidatesArgs()
            {
                //Arrange
                var target = new AzureStorageFactory();

                //Act Assert
                Assert.That(() => target.GetAzureQueue<CloudQueueMessage>(null, "foo"), Throws.ArgumentNullException);
                Assert.That(() => target.GetAzureQueue<CloudQueueMessage>(mockAccount, string.Empty), Throws.ArgumentNullException);
            }

            [Test]
            public void Sucesss()
            {
                //Arrange
                var target = new AzureStorageFactory();

                //Act
                var result = target.GetAzureQueue<CloudQueueMessage>(mockAccount, "foo");

                //Assert
                Assert.IsNotNull(result);
            }
        }
    }
}
