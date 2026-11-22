using TypesEnumsNullablesConversions.Enums;

namespace TypesEnumsNullablesConversions.Tests
{
    // Task 1.10 - DaysOfWeek + [Flags] FilePermission
    public class EnumTests
    {
        [Theory]
        [InlineData("0", "Day 0 is Sunday")]
        [InlineData("3", "Day 3 is Wednesday")]
        [InlineData("6", "Day 6 is Saturday")]
        public void Run_ValidDayNumber_PrintsDayName(string input, string expected)
        {
            string output = ConsoleCapture.Run(EnumDemo.Run, input + Environment.NewLine);

            Assert.Contains(expected, output);
        }

        [Theory]
        [InlineData("7")]
        [InlineData("-1")]
        [InlineData("abc")]
        [InlineData("")]
        public void Run_InvalidDayNumber_PrintsValidationMessage(string input)
        {
            string output = ConsoleCapture.Run(EnumDemo.Run, input + Environment.NewLine);

            Assert.Contains($"'{input}' is not a valid day number (0-6).", output);
        }

        [Fact]
        public void Run_NoInput_PrintsValidationMessage()
        {
            string output = ConsoleCapture.Run(EnumDemo.Run, string.Empty);

            Assert.Contains("is not a valid day number (0-6).", output);
        }

        [Fact]
        public void Run_PrintsFlagsCombinationAndChecks()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(EnumDemo.Run, "1" + Environment.NewLine));

            Assert.Contains("Combined permissions: Read, Write", lines);
            Assert.Contains("Numeric value: 3", lines);
            Assert.Contains("Can write?   True", lines);
            Assert.Contains("Can execute? False", lines);
            Assert.Contains("After adding Execute: Read, Write, Execute", lines);
        }

        [Fact]
        public void DaysOfWeek_ValuesStartAtZero()
        {
            Assert.Equal(0, (int)DaysOfWeek.Sunday);
            Assert.Equal(6, (int)DaysOfWeek.Saturday);
            Assert.Equal(DaysOfWeek.Wednesday, (DaysOfWeek)3);
        }

        [Fact]
        public void FilePermission_ValuesArePowersOfTwo()
        {
            Assert.Equal(0, (int)FilePermission.None);
            Assert.Equal(1, (int)FilePermission.Read);
            Assert.Equal(2, (int)FilePermission.Write);
            Assert.Equal(4, (int)FilePermission.Execute);
        }

        [Fact]
        public void FilePermission_CombineWithOr_TestWithAnd()
        {
            FilePermission perms = FilePermission.Read | FilePermission.Execute;

            Assert.Equal(5, (int)perms);
            Assert.Equal(FilePermission.Read, perms & FilePermission.Read);
            Assert.Equal(FilePermission.None, perms & FilePermission.Write);
            Assert.True(perms.HasFlag(FilePermission.Execute));
            Assert.Equal("Read, Execute", perms.ToString());
        }
    }
}
