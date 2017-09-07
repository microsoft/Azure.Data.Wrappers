using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Data.Wrappers.Test
{
    public class TestHelpers
    {
        public const string DevConnectionString = "UseDevelopmentStorage=true;";
        public const string DevAccountName = "devstoreaccount1";
        public const string DevAccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

        public static async Task<Container> GetTestContainer()  
        {
            var container = new Container(generateUniqueName(), DevConnectionString);
            await container.CreateIfNotExists();
            return container;
        }

        public static string generateUniqueName()
        {
            return 'a' + Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
