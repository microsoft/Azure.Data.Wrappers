namespace Azure.Data.Wrappers.Test
{
    using NUnit.Common;
    using NUnitLite;
    using System;
    using System.Reflection;

    public class Program
    {
        public static void Main(string[] args)
        {
            var writter = new ExtendedTextWrapper(Console.Out);
            new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, writter, Console.In);

            Console.WriteLine("Testing Completed.");
            Console.Read();
        }
    }
}