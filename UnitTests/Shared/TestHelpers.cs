using System.Globalization;

// Tests redirect the global Console, so they must not run in parallel.
[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

namespace TestUtilities
{
    /// <summary>
    /// Captures everything written to Console while an action runs, optionally feeding
    /// Console input. Uses en-US culture so number/percent formatting is predictable.
    /// </summary>
    public static class ConsoleCapture
    {
        private static readonly CultureInfo TestCulture = CultureInfo.GetCultureInfo("en-US");

        public static string Run(Action action, string? input = null)
        {
            TextWriter originalOut = Console.Out;
            TextReader originalIn = Console.In;
            CultureInfo originalCulture = CultureInfo.CurrentCulture;

            using var writer = new StringWriter();
            try
            {
                Console.SetOut(writer);
                if (input != null)
                {
                    Console.SetIn(new StringReader(input));
                }
                CultureInfo.CurrentCulture = TestCulture;
                action();
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                CultureInfo.CurrentCulture = originalCulture;
            }
            return writer.ToString();
        }

        /// <summary>Runs a function under en-US culture (for methods that format numbers).</summary>
        public static T WithCulture<T>(Func<T> func)
        {
            CultureInfo originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = TestCulture;
                return func();
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        /// <summary>Splits captured output into trimmed-end lines.</summary>
        public static string[] Lines(string output) =>
            output.Replace("\r\n", "\n").Split('\n').Select(l => l.TrimEnd()).ToArray();
    }
}
