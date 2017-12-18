using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Azure.Data.Wrappers
{
    public class SanitizedKeysTableEntity : TableEntity, ISupportsSanitizedKeys
    {
        public SanitizedKeysTableEntity()
        {
            this.ReplacementValue = string.Empty;
        }

        private const string DisallowedCharsInTableKeys = @"[\\\\#%+/?\u0000-\u001F\u007F-\u009F]";

        public string PartitionKeyUnsanitized { get; private set; }
        public string RowKeyUnsanitized { get; private set; }
        public string ReplacementValue { get; private set; }

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
        public bool SanitizeKeys(ISanitizationProvider sanitizationProvider)
        {
            if (sanitizationProvider == null) throw new ArgumentNullException(nameof(sanitizationProvider));

            this.PartitionKey = sanitizationProvider.Sanitize(this.PartitionKey);
            this.RowKey = sanitizationProvider.Sanitize(this.RowKey);

            return (this.PartitionKeyUnsanitized.Equals(this.PartitionKey) && this.RowKeyUnsanitized.Equals(this.RowKey)) ? false : true;
        }
    }
}
