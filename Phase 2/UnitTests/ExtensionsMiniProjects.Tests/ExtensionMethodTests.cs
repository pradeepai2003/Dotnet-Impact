using ExtensionsMiniProjects.AnonymousTypes;
using ExtensionsMiniProjects.Extensions;
using ExtensionsMiniProjects.MiniProjects.Payroll;

namespace ExtensionsMiniProjects.Tests
{
    // Task 2.14 - string.ToTitleCase()
    public class ToTitleCaseTests
    {
        [Theory]
        [InlineData("hello world", "Hello World")]
        [InlineData("hELLO wORLD", "Hello World")]
        [InlineData("ALREADY TITLE", "Already Title")]
        [InlineData("a", "A")]
        [InlineData("x y z", "X Y Z")]
        [InlineData("hello   world", "Hello   World")]        // multiple spaces kept
        [InlineData("  leading and trailing  ", "  Leading And Trailing  ")]
        [InlineData("tab\tseparated", "Tab\tSeparated")]
        [InlineData("c# is FUN", "C# Is Fun")]
        [InlineData("123 abc", "123 Abc")]
        public void ToTitleCase_Examples(string input, string expected)
        {
            Assert.Equal(expected, input.ToTitleCase());
        }

        [Fact]
        public void ToTitleCase_Empty_ReturnsEmpty() => Assert.Equal("", "".ToTitleCase());

        [Fact]
        public void ToTitleCase_Null_ReturnsEmpty()
        {
            string? text = null;
            Assert.Equal("", text.ToTitleCase());
        }

        [Fact]
        public void ToTitleCase_WhitespaceOnly_IsUnchanged() => Assert.Equal("   ", "   ".ToTitleCase());
    }

    // Task 2.14 - List<T>.IsNullOrEmpty()
    public class IsNullOrEmptyTests
    {
        [Fact]
        public void NullList_IsTrue_WithoutNullReferenceException()
        {
            List<int>? list = null;
            Assert.True(list.IsNullOrEmpty());
        }

        [Fact]
        public void EmptyList_IsTrue() => Assert.True(new List<string>().IsNullOrEmpty());

        [Fact]
        public void OneItem_IsFalse() => Assert.False(new List<int> { 0 }.IsNullOrEmpty());

        [Fact]
        public void ListOfNulls_IsFalse() => Assert.False(new List<string?> { null }.IsNullOrEmpty());

        [Fact]
        public void ClearedList_IsTrue()
        {
            var list = new List<int> { 1, 2 };
            list.Clear();
            Assert.True(list.IsNullOrEmpty());
        }
    }

    // Task 2.14 - int.ToWords()
    public class ToWordsTests
    {
        [Theory]
        [InlineData(0, "zero")]
        [InlineData(1, "one")]
        [InlineData(9, "nine")]
        [InlineData(10, "ten")]
        [InlineData(11, "eleven")]
        [InlineData(15, "fifteen")]
        [InlineData(19, "nineteen")]
        [InlineData(20, "twenty")]
        [InlineData(21, "twenty-one")]
        [InlineData(45, "forty-five")]
        [InlineData(90, "ninety")]
        [InlineData(99, "ninety-nine")]
        [InlineData(100, "one hundred")]
        [InlineData(101, "one hundred and one")]
        [InlineData(110, "one hundred and ten")]
        [InlineData(115, "one hundred and fifteen")]
        [InlineData(120, "one hundred and twenty")]
        [InlineData(342, "three hundred and forty-two")]
        [InlineData(500, "five hundred")]
        [InlineData(999, "nine hundred and ninety-nine")]
        public void ToWords_Examples(int number, string expected)
        {
            Assert.Equal(expected, number.ToWords());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1000)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void ToWords_OutOfRange_Throws(int number)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => number.ToWords());
            Assert.Equal("number", ex.ParamName);
        }
    }

    public class ExtensionsDemoTests
    {
        [Fact]
        public void Run_ShowsAllThreeExtensions()
        {
            string output = ConsoleCapture.Run(ExtensionsDemo.Run);

            Assert.Contains("ToTitleCase(\"hELLO   wORLD\") = \"Hello   World\"", output);
            Assert.Contains("ToTitleCase(null) = \"\"", output);
            Assert.Contains("null list IsNullOrEmpty   = True", output);
            Assert.Contains("[1, 2] IsNullOrEmpty      = False", output);
            Assert.Contains("999.ToWords() = nine hundred and ninety-nine", output);
        }
    }

    // Task 2.14 - anonymous types
    public class AnonymousTypeTests
    {
        [Fact]
        public void GetAnnualSalaries_ProjectsEmployeeNameAndAnnualSalary()
        {
            var result = AnonymousTypeDemo.GetAnnualSalaries(PayrollService.CreateSample().Employees);

            Assert.Equal(new[]
            {
                ("Asha", 960000m),     // full-time 80,000 x 12
                ("Ravi", 480000m),     // part-time 500 x 80h x 12
                ("Meena", 720000m),
                ("John", 600000m),     // contract 50,000 x 12
                ("Karthik", 660000m),
                ("Sneha", 288000m),
            }, result);
        }

        [Fact]
        public void GetAnnualSalaries_Empty_ReturnsEmpty()
        {
            Assert.Empty(AnonymousTypeDemo.GetAnnualSalaries(new List<Employee>()));
        }

        [Fact]
        public void GetAnnualSalaries_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => AnonymousTypeDemo.GetAnnualSalaries(null!));
        }

        [Fact]
        public void AnonymousType_HasCompilerGeneratedName()
        {
            var anonymous = new { Name = "Asha", AnnualSalary = 1m };

            Assert.Contains("AnonymousType", anonymous.GetType().Name);
            Assert.Equal(new { Name = "Asha", AnnualSalary = 1m }, anonymous);   // value equality
        }

        [Fact]
        public void Run_PrintsAnnualSalaries()
        {
            string output = ConsoleCapture.Run(AnonymousTypeDemo.Run);

            Assert.Contains("Asha     annual salary 960,000.00", output);
            Assert.Contains("Sneha    annual salary 288,000.00", output);
            Assert.Contains("Compiler-generated type name: <>f__AnonymousType", output);
        }
    }
}
