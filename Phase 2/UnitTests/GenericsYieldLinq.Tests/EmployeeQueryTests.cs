using GenericsYieldLinq.Linq;

namespace GenericsYieldLinq.Tests
{
    // Task 2.13 - every query twice; both must return identical results
    public class EmployeeQueryTests
    {
        private static readonly List<Employee> Employees = EmployeeData.Sample();
        private static readonly DateTime AsOf = new(2026, 11, 22);

        [Fact]
        public void Sample_HasAtLeastTenRows()
        {
            Assert.True(Employees.Count >= 10);
        }

        [Fact]
        public void HighEarners_QueryEqualsMethod_AndIsCorrect()
        {
            var query = EmployeeQueries.HighEarnersQuery(Employees);
            var method = EmployeeQueries.HighEarnersMethod(Employees);

            Assert.Equal(query, method);
            Assert.Equal(new[] { "Asha", "Ravi", "Karthik", "John", "Arif", "Divya", "Suresh" }, query.Select(e => e.Name));
            Assert.All(query, e => Assert.True(e.Salary > 50000m));
        }

        [Fact]
        public void HighEarners_CustomThreshold()
        {
            var query = EmployeeQueries.HighEarnersQuery(Employees, 90000m);

            Assert.Equal(query, EmployeeQueries.HighEarnersMethod(Employees, 90000m));
            Assert.Equal("Asha", Assert.Single(query).Name);
        }

        [Fact]
        public void BySalaryDesc_QueryEqualsMethod_AndIsSorted()
        {
            var query = EmployeeQueries.BySalaryDescQuery(Employees);
            var method = EmployeeQueries.BySalaryDescMethod(Employees);

            Assert.Equal(query, method);
            Assert.Equal("Asha", query[0].Name);
            Assert.Equal("Vikram", query[^1].Name);
            Assert.Equal(query.OrderByDescending(e => e.Salary).Select(e => e.Salary), query.Select(e => e.Salary));
        }

        [Fact]
        public void BySalaryDesc_TiesAreOrderedByName()
        {
            var tied = new List<Employee>
            {
                new("Zara", "X", 100m, AsOf),
                new("Adam", "X", 100m, AsOf),
            };

            Assert.Equal(new[] { "Adam", "Zara" }, EmployeeQueries.BySalaryDescQuery(tied).Select(e => e.Name));
            Assert.Equal(new[] { "Adam", "Zara" }, EmployeeQueries.BySalaryDescMethod(tied).Select(e => e.Name));
        }

        [Fact]
        public void DepartmentStats_QueryEqualsMethod_AndIsCorrect()
        {
            var query = EmployeeQueries.DepartmentStatsQuery(Employees);
            var method = EmployeeQueries.DepartmentStatsMethod(Employees);

            Assert.Equal(query, method);
            Assert.Equal(new[]
            {
                new DepartmentStat("Engineering", 4, 75750m),
                new DepartmentStat("Finance", 3, 54666.67m),
                new DepartmentStat("HR", 2, 49500m),
                new DepartmentStat("Marketing", 3, 50000m),
            }, query);
        }

        [Fact]
        public void Experience_QueryEqualsMethod_AndIsCorrect()
        {
            var query = EmployeeQueries.ExperienceQuery(Employees, AsOf);
            var method = EmployeeQueries.ExperienceMethod(Employees, AsOf);

            Assert.Equal(query, method);
            Assert.Equal(12, query.Count);
            Assert.Equal(("Arif", 11), query[0]);
            Assert.Contains(("Vikram", 2), query);
            Assert.Contains(("Suresh", 12), query);
        }

        [Theory]
        [InlineData("2020-01-01", "2026-11-22", 6)]
        [InlineData("2020-11-22", "2026-11-22", 6)]   // exactly on the anniversary
        [InlineData("2020-11-23", "2026-11-22", 5)]   // one day before the anniversary
        [InlineData("2026-12-01", "2026-11-22", 0)]   // joined in the future -> never negative
        public void YearsBetween_CountsCompletedYears(string from, string to, int expected)
        {
            Assert.Equal(expected, EmployeeQueries.YearsBetween(DateTime.Parse(from), DateTime.Parse(to)));
        }

        [Fact]
        public void EmptyInput_AllQueriesReturnEmpty()
        {
            var none = new List<Employee>();

            Assert.Empty(EmployeeQueries.HighEarnersQuery(none));
            Assert.Empty(EmployeeQueries.HighEarnersMethod(none));
            Assert.Empty(EmployeeQueries.DepartmentStatsQuery(none));
            Assert.Empty(EmployeeQueries.DepartmentStatsMethod(none));
            Assert.Empty(EmployeeQueries.ExperienceQuery(none, AsOf));
            Assert.Empty(EmployeeQueries.ExperienceMethod(none, AsOf));
        }

        [Fact]
        public void LinqDemo_Run_ShowsAllFourQueriesMatching()
        {
            string output = ConsoleCapture.Run(LinqDemo.Run);

            Assert.Equal(4, output.Split("query == method: True").Length - 1);
            Assert.Contains("Engineering  count 4, avg 75,750.00", output);
            Assert.Contains("Arif 11y", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => GenericsYieldLinq.Program.Main(Array.Empty<string>()));

            int t11 = output.IndexOf("=== Task 2.11", StringComparison.Ordinal);
            int t12 = output.IndexOf("=== Task 2.12", StringComparison.Ordinal);
            int t13 = output.IndexOf("=== Task 2.13", StringComparison.Ordinal);

            Assert.True(t11 >= 0 && t11 < t12 && t12 < t13);
        }
    }
}
