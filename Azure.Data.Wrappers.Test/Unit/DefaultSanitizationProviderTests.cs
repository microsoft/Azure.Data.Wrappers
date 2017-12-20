namespace Azure.Data.Wrappers.Test.Unit
{
    using Azure.Data.Wrappers.Sanitization.Providers;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class DefaultSanitizationProviderTests
    {
        [Test]
        public void Sanitize_NoSanitizationRequired()
        {
            var target = new DefaultSanitizationProvider();
            var output = target.Sanitize("Hello World");

            Assert.AreEqual("Hello World", output);
        }

        [Test]
        public void Sanitize_SanitizationRequired()
        {
            var target = new DefaultSanitizationProvider();
            var output = target.Sanitize(@"#%/?");

            Assert.AreEqual(string.Empty, output);
        }
        [Test]
        public void Sanitize_InvalidReplacementCharacter()
        {
            var target = new CustomDefaultSanitizationProvider();

            Assert.That(() => target.Sanitize(@"Hello World"), Throws.TypeOf<InvalidOperationException>());
        }
    }

    public class CustomDefaultSanitizationProvider : DefaultSanitizationProvider
    {
        protected override string GetReplacementValue(string input)
        {
            return "%";
        }
    }
}
