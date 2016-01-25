namespace King.Azure.Integration.Test
{
    using NUnit.Framework;
    using System.Configuration;
    using System.Diagnostics;

    [SetUpFixture]
    public class Initialization
    {
        [OneTimeSetUp]
        public static void SetUp()
        {
            var emulator = ConfigurationManager.AppSettings["AzureEmulator"];

            using (var process = new Process())
            {
                process.StartInfo = CreateProcessStartInfo(emulator, "start");
                process.Start();

                process.WaitForExit();
            }
        }

        private static ProcessStartInfo CreateProcessStartInfo(string fileName, string arguments)
        {
            return new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
        }
    }
}