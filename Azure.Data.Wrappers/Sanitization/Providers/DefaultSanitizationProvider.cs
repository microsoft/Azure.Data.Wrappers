namespace Azure.Data.Wrappers.Sanitization.Providers
{
    using System;
    using System.Text.RegularExpressions;

    public class DefaultSanitizationProvider : ISanitizationProvider
    {
        private const string DisallowedCharsInTableKeys = @"[\\\\#%+/?\u0000-\u001F\u007F-\u009F]";

        /// <summary>
        /// Use this to replace invalid characters from Azure table keys.  The following characters are not allowed in Azure Table Keys:
        /// The forward slash (/) character
        /// The backslash(\) character
        /// The number sign(#) character
        /// The question mark (?) character
        /// The % sign(%) character
        /// Control characters from U+0000 to U+001F, including:
        /// The horizontal tab (\t) character
        /// The linefeed(\n) character
        /// The carriage return (\r) character
        /// Control characters from U+007F to U+009F
        /// </summary>
        /// <param name="input">input string to sanitize</param>
        /// <param name="replacementValue">value to use when replacing a character.  Note that using a fixed replacement value could cause collisions and that could cause a RowKey failure.  </param>
        /// <seealso cref="https://docs.microsoft.com/en-us/rest/api/storageservices/Understanding-the-Table-Service-Data-Model?redirectedfrom=MSDN"/>
        /// <returns>sanitized string</returns>
        public virtual string Sanitize(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            string replacementValue = this.GetReplacenentValue(input);

            if (Regex.IsMatch(replacementValue, DisallowedCharsInTableKeys)) throw new InvalidOperationException("Replacement Value cannot contain a character in the disallowed list.");

            return this.Replace(input, replacementValue);
        }

        protected virtual string GetReplacenentValue(string input)
        {
            return string.Empty;
        }

        protected virtual string Replace(string input, string replacementValue)
        {
            return Regex.Replace(input, DisallowedCharsInTableKeys, replacementValue);
        }
    }
}
