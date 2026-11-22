using System.Globalization;
using ModernSyntaxClassAnatomy.PatternMatching;

namespace ModernSyntaxClassAnatomy.Tests
{
    // Task 1.15 - type, relational and property patterns
    public class PatternMatchingTests
    {
        public static TheoryData<object?, string> DescribeCases => new()
        {
            { 42, "int: 42 (doubled = 84)" },
            { "pattern", "string: \"pattern\" (length 7)" },
            { 3.14, "double: 3.14 (rounded = 3)" },
            { null, "null: no value supplied" },
            { true, "other type: Boolean" },
            { 'x', "other type: Char" },
        };

        [Theory]
        [MemberData(nameof(DescribeCases))]
        public void Describe_MatchesEachTypeBranch(object? value, string expected)
        {
            string result = ConsoleCapture.WithCulture(() => PatternDemo.Describe(value));

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(-1, "Invalid")]
        [InlineData(101, "Invalid")]
        [InlineData(100, "A")]
        [InlineData(90, "A")]
        [InlineData(89, "B")]
        [InlineData(80, "B")]
        [InlineData(79, "C")]
        [InlineData(70, "C")]
        [InlineData(69, "D")]
        [InlineData(50, "D")]
        [InlineData(49, "F")]
        [InlineData(0, "F")]
        public void GetGrade_UsesRelationalPatternBoundaries(int marks, string expected)
        {
            Assert.Equal(expected, PatternDemo.GetGrade(marks));
        }

        [Theory]
        [InlineData(OrderStatus.Cancelled, 9000, "0")]
        [InlineData(OrderStatus.New, 5000, "0.15")]
        [InlineData(OrderStatus.New, 4999, "0.05")]
        [InlineData(OrderStatus.Shipped, 3000, "0.02")]
        [InlineData(OrderStatus.Delivered, 2001, "0.02")]
        [InlineData(OrderStatus.Delivered, 2000, "0")]
        public void GetDiscount_UsesPropertyPatterns(OrderStatus status, int amount, string expected)
        {
            var order = new Order { Status = status, Amount = amount };

            Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), PatternDemo.GetDiscount(order));
        }

        [Fact]
        public void Order_PropertiesCanBeSet()
        {
            var order = new Order { Status = OrderStatus.Shipped, Amount = 12.5m };

            Assert.Equal(OrderStatus.Shipped, order.Status);
            Assert.Equal(12.5m, order.Amount);
        }

        [Fact]
        public void Run_PrintsEveryPatternPath()
        {
            string output = ConsoleCapture.Run(PatternDemo.Run);

            Assert.Contains("int: 42 (doubled = 84)", output);
            Assert.Contains("null: no value supplied", output);
            Assert.Contains("Marks  95 -> Grade A", output);
            Assert.Contains("Marks 120 -> Grade Invalid", output);
            Assert.Contains("6000 -> discount 15%", output);
            Assert.Contains("1500 -> discount 5%", output);
            Assert.Contains("3000 -> discount 2%", output);
            Assert.Contains("9000 -> discount 0%", output);
        }
    }
}
