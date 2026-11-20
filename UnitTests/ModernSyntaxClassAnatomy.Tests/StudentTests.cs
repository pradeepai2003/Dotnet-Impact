using ModernSyntaxClassAnatomy.ClassAnatomy;

namespace ModernSyntaxClassAnatomy.Tests
{
    // Task 1.16 - validated properties, constructor chaining, overloads, const vs readonly
    public class StudentTests
    {
        [Fact]
        public void Constructor_SetsNameAndAge()
        {
            var student = new Student("Priya", 15);

            Assert.Equal("Priya", student.Name);
            Assert.Equal(15, student.Age);
        }

        [Fact]
        public void ChainedConstructor_AppliesDefaultAge18()
        {
            Student? student = null;
            string output = ConsoleCapture.Run(() => student = new Student("Arjun"));

            Assert.NotNull(student);
            Assert.Equal("Arjun", student!.Name);
            Assert.Equal(18, student.Age);
            Assert.Contains("(chained ctor used: default age 18 applied for Arjun)", output);
        }

        [Fact]
        public void StudentId_IsUniqueAndIncreasing()
        {
            var first = new Student("A", 10);
            var second = new Student("B", 10);

            Assert.Equal(first.StudentId + 1, second.StudentId);
        }

        [Fact]
        public void EnrolledOn_IsToday()
        {
            var student = new Student("A", 10);

            Assert.Equal(DateTime.Today, student.EnrolledOn);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(50)]
        [InlineData(100)]
        public void Age_WithinRange_IsAccepted(int age)
        {
            var student = new Student("A", 10) { Age = age };

            Assert.Equal(age, student.Age);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(101)]
        [InlineData(-1)]
        public void Constructor_OutOfRangeAge_Throws(int age)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Student("A", age));

            Assert.Equal("Age", ex.ParamName);
            Assert.Equal(age, ex.ActualValue);
        }

        [Fact]
        public void SetAge_OutOfRange_ThrowsAndKeepsOldValue()
        {
            var student = new Student("Priya", 15);

            Assert.Throws<ArgumentOutOfRangeException>(() => student.Age = 150);
            Assert.Equal(15, student.Age);
        }

        [Theory]
        [InlineData(95, "A")]
        [InlineData(90, "A")]
        [InlineData(89, "B")]
        [InlineData(75, "B")]
        [InlineData(74, "C")]
        [InlineData(60, "C")]
        [InlineData(59, "D")]
        [InlineData(40, "D")]
        [InlineData(39, "F")]
        public void CalculateGrade_SingleScore(int marks, string expected)
        {
            Assert.Equal(expected, new Student("A", 10).CalculateGrade(marks));
        }

        [Theory]
        [InlineData(45, 60, "B")]   // 75%
        [InlineData(30, 60, "D")]   // 50%
        [InlineData(19, 20, "A")]   // 95%
        public void CalculateGrade_ObtainedOutOfTotal(int obtained, int total, string expected)
        {
            Assert.Equal(expected, new Student("A", 10).CalculateGrade(obtained, total));
        }

        [Fact]
        public void CalculateGrade_Array_UsesAverage()
        {
            // (70 + 80 + 65) / 3 = 71.67 -> C
            Assert.Equal("C", new Student("A", 10).CalculateGrade(new[] { 70, 80, 65 }));
        }

        [Fact]
        public void Const_IsSharedViaTheClass()
        {
            Assert.Equal("Greenwood High", Student.SchoolName);
            Assert.True(typeof(Student).GetField(nameof(Student.SchoolName))!.IsLiteral);
        }

        [Fact]
        public void Readonly_FieldIsInitOnly()
        {
            var field = typeof(Student).GetField(nameof(Student.StudentId))!;

            Assert.True(field.IsInitOnly);
            Assert.False(field.IsLiteral);
        }

        [Fact]
        public void TryChangeId_DoesNotChangeReadonlyId()
        {
            var student = new Student("A", 10);
            int id = student.StudentId;

            student.TryChangeId();

            Assert.Equal(id, student.StudentId);
        }

        [Fact]
        public void ToString_IncludesAllDetails()
        {
            var student = new Student("Priya", 15);

            Assert.Equal(
                $"#{student.StudentId} Priya, Age 15, Greenwood High, enrolled {DateTime.Today:yyyy-MM-dd}",
                student.ToString());
        }

        [Fact]
        public void StudentDemo_Run_ShowsValidationChainingAndOverloads()
        {
            string output = ConsoleCapture.Run(StudentDemo.Run);

            Assert.Equal(2, CountOccurrences(output, "Rejected: Age must be between 5 and 100."));
            Assert.Contains("s1.Age is still 15", output);
            Assert.Contains("(chained ctor used: default age 18 applied for Arjun)", output);
            Assert.Contains("CalculateGrade(92)           = A", output);
            Assert.Contains("CalculateGrade(45, 60)       = B", output);
            Assert.Contains("CalculateGrade([70, 80, 65]) = C", output);
            Assert.Contains("const    SchoolName = Greenwood High", output);
        }

        private static int CountOccurrences(string text, string value)
        {
            int count = 0, index = 0;
            while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += value.Length;
            }
            return count;
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => ModernSyntaxClassAnatomy.Program.Main(Array.Empty<string>()));

            string[] expectedInOrder =
            {
                "=== Task 1.13: var vs dynamic ===",
                "=== Task 1.14: Tuples & deconstruction ===",
                "=== Task 1.15: Pattern matching ===",
                "=== Task 1.16: Class anatomy ===",
                "readonly StudentId",
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
