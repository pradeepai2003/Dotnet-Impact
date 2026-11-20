using TypesEnumsNullablesConversions.Conversions;
using TypesEnumsNullablesConversions.Nullables;

namespace TypesEnumsNullablesConversions.Tests
{
    // Task 1.11 - nullables and ??
    public class NullableTests
    {
        [Fact]
        public void ApplyDiscount_Null_UsesFivePercentDefault()
        {
            double result = ConsoleCapture.WithCulture(() => NullableDemo.ApplyDiscount(1000, null));

            Assert.Equal(950, result);
        }

        [Theory]
        [InlineData(1000, 0.20, 800)]
        [InlineData(500, 0.10, 450)]
        [InlineData(200, 0.0, 200)]
        public void ApplyDiscount_Value_UsesThatValue(double price, double discount, double expected)
        {
            double result = ConsoleCapture.WithCulture(() => NullableDemo.ApplyDiscount(price, discount));

            Assert.Equal(expected, result, precision: 6);
        }

        [Fact]
        public void ApplyDiscount_PrintsAppliedRate()
        {
            string output = ConsoleCapture.Run(() => NullableDemo.ApplyDiscount(1000, null));

            Assert.Contains("Applying 5% discount on 1000", output);
        }

        [Fact]
        public void Run_PrintsHasValueAndDiscountResults()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(NullableDemo.Run));

            Assert.Contains("marks.HasValue = False", lines);
            Assert.Contains("marks.HasValue = True, Value = 85", lines);
            Assert.Contains("Discount null -> 950", lines);
            Assert.Contains("Discount 0.20 -> 800", lines);
        }
    }

    // Task 1.12 - implicit/explicit conversions, is, as, Convert, TryParse
    public class ConversionTests
    {
        [Fact]
        public void Run_ShowsImplicitWideningChain()
        {
            string output = ConsoleCapture.Run(ConversionDemo.Run);

            Assert.Contains("Implicit: int 123456 -> long 123456 -> float 123456 -> double 123456", output);
        }

        [Fact]
        public void Run_ShowsExplicitCastTruncates()
        {
            string output = ConsoleCapture.Run(ConversionDemo.Run);

            Assert.Contains("Explicit: double 99.99 -> int 99", output);
        }

        [Fact]
        public void Run_ShowsIsAndAsResults()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(ConversionDemo.Run));

            Assert.Contains("is:   value is string? True", lines);
            Assert.Contains("is:   value is int?    False", lines);
            Assert.Contains("is:   pattern matched string \"123\"", lines);
            Assert.Contains("as:   as string = \"123\", as int? = null", lines);
        }

        [Fact]
        public void Run_ShowsConvertAndTryParseResults()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(ConversionDemo.Run));

            Assert.Contains("Convert.ToInt32(\"123\") = 123", lines);
            Assert.Contains("Convert.ToInt32(null)  = 0", lines);
            Assert.Contains("Convert.ToInt32(\"abc\") threw FormatException", lines);
            Assert.Contains("int.TryParse(\"456\") succeeded: 456", lines);
            Assert.Contains("int.TryParse(\"abc\") failed, no exception", lines);
            Assert.Contains("int.TryParse(\"\") failed, no exception", lines);
        }

        [Fact]
        public void ExplicitCast_TruncatesInsteadOfRounding()
        {
            double price = 99.99;

            Assert.Equal(99, (int)price);
            Assert.Equal(-2, (int)-2.7);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(
                () => TypesEnumsNullablesConversions.Program.Main(Array.Empty<string>()),
                "3" + Environment.NewLine);

            string[] expectedInOrder =
            {
                "=== Task 1.9: Value vs Reference types ===",
                "=== Task 1.10: Enums & [Flags] ===",
                "Day 3 is Wednesday",
                "=== Task 1.11: Nullables & ?? ===",
                "=== Task 1.12: Conversions ===",
                "int.TryParse(\"\") failed, no exception",
            };

            int position = -1;
            foreach (string expected in expectedInOrder)
            {
                int next = output.IndexOf(expected, StringComparison.Ordinal);
                Assert.True(next > position, $"Expected \"{expected}\" after position {position}.");
                position = next;
            }
        }
    }
}
