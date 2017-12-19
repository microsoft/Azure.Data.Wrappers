using Azure.Data.Wrappers.Sanitization.Providers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Data.Wrappers.Test.Unit
{
    [TestFixture]
    public class DefaultSanitizationProviderTests
    {
        [Test]
        public void Sanitize_NoSanitizationRequired()
        {
            var target = new DefaultSanitizationProvider();
            var output = target.Sanitize("Hello World");

            Assert.AreEqual(output, )

        }

    }
}
