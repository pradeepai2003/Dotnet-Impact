using ExtensionsMiniProjects.MiniProjects.Payroll;

namespace ExtensionsMiniProjects.Tests
{
    // Mini Q4 - Employee Payroll
    public class PayrollEmployeeTests
    {
        [Fact]
        public void FullTime_SalaryAndTax()
        {
            var e = new FullTimeEmployee("Asha", "Engineering", 80000m);

            Assert.Equal(80000m, e.CalculateSalary());
            Assert.Equal(0.10m, e.TaxRate);
            Assert.Equal(8000m, e.CalculateTax());
            Assert.Equal("Full-time", e.EmployeeType);
        }

        [Fact]
        public void FullTime_CustomTaxRate_RoundsToTwoDecimals()
        {
            var e = new FullTimeEmployee("Asha", "Engineering", 33333.33m, 0.15m);

            Assert.Equal(5000.00m, e.CalculateTax());   // 4999.9995 -> 5000.00
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void FullTime_BoundaryTaxRates_AreAllowed(int rate)
        {
            Assert.Equal(1000m * rate, new FullTimeEmployee("A", "D", 1000m, rate).CalculateTax());
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(1.01)]
        public void FullTime_InvalidTaxRate_Throws(double rate)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FullTimeEmployee("A", "D", 1000m, (decimal)rate));
        }

        [Fact]
        public void FullTime_NegativeSalary_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FullTimeEmployee("A", "D", -1m));
        }

        [Fact]
        public void PartTime_Salary_IsRateTimesHours()
        {
            var e = new PartTimeEmployee("Ravi", "Engineering", 500m, 80m);

            Assert.Equal(40000m, e.CalculateSalary());
            Assert.Equal("Part-time", e.EmployeeType);
        }

        [Fact]
        public void PartTime_ZeroHours_IsZeroSalary()
        {
            Assert.Equal(0m, new PartTimeEmployee("Ravi", "D", 500m, 0m).CalculateSalary());
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(10, -1)]
        public void PartTime_NegativeValues_Throw(int rate, int hours)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PartTimeEmployee("A", "D", rate, hours));
        }

        [Fact]
        public void Contract_Salary_IsAmountSpreadOverMonths()
        {
            var e = new ContractEmployee("John", "Finance", 300000m, 6);

            Assert.Equal(50000m, e.CalculateSalary());
            Assert.Equal("Contract", e.EmployeeType);
        }

        [Fact]
        public void Contract_Salary_IsRounded()
        {
            Assert.Equal(33333.33m, new ContractEmployee("A", "D", 100000m, 3).CalculateSalary());
        }

        [Theory]
        [InlineData(1000, 0)]
        [InlineData(-1, 6)]
        public void Contract_InvalidValues_Throw(int amount, int months)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ContractEmployee("A", "D", amount, months));
        }

        [Theory]
        [InlineData("", "Finance")]
        [InlineData("Asha", " ")]
        public void Employee_BlankNameOrDepartment_Throws(string name, string department)
        {
            Assert.Throws<ArgumentException>(() => new PartTimeEmployee(name, department, 1m, 1m));
        }

        [Fact]
        public void OnlyFullTime_ImplementsITaxable()
        {
            Assert.True(typeof(ITaxable).IsAssignableFrom(typeof(FullTimeEmployee)));
            Assert.False(typeof(ITaxable).IsAssignableFrom(typeof(PartTimeEmployee)));
            Assert.False(typeof(ITaxable).IsAssignableFrom(typeof(ContractEmployee)));
        }

        [Fact]
        public void ToString_ShowsNameTypeAndDepartment()
        {
            Assert.Equal("John (Contract, Finance)", new ContractEmployee("John", "Finance", 1m, 1).ToString());
        }
    }

    public class PayrollServiceTests
    {
        private readonly PayrollService _payroll = PayrollService.CreateSample();

        [Fact]
        public void TotalPayroll_UsesPolymorphism()
        {
            // 80,000 + 40,000 + 60,000 + 50,000 + 55,000 + 24,000
            Assert.Equal(309000m, _payroll.TotalPayroll());
        }

        [Fact]
        public void TotalTax_OnlyCountsITaxableEmployees()
        {
            // 8,000 + 6,000 + 5,500 (full-time only)
            Assert.Equal(19500m, _payroll.TotalTax());
        }

        [Fact]
        public void TaxFor_TaxableAndNonTaxable()
        {
            Employee fullTime = _payroll.Employees.First(e => e.Name == "Asha");
            Employee partTime = _payroll.Employees.First(e => e.Name == "Ravi");
            Employee contract = _payroll.Employees.First(e => e.Name == "John");

            Assert.Equal(8000m, _payroll.TaxFor(fullTime));
            Assert.Equal(0m, _payroll.TaxFor(partTime));
            Assert.Equal(0m, _payroll.TaxFor(contract));
        }

        [Fact]
        public void NetPayFor_SubtractsTaxOnlyWhenTaxable()
        {
            Assert.Equal(72000m, _payroll.NetPayFor(_payroll.Employees.First(e => e.Name == "Asha")));
            Assert.Equal(40000m, _payroll.NetPayFor(_payroll.Employees.First(e => e.Name == "Ravi")));
        }

        [Fact]
        public void ByDepartment_GroupsCountsAndTotals()
        {
            Assert.Equal(new[]
            {
                new DepartmentPayroll("Engineering", 2, 120000m),
                new DepartmentPayroll("Finance", 2, 110000m),
                new DepartmentPayroll("Marketing", 2, 79000m),
            }, _payroll.ByDepartment());
        }

        [Fact]
        public void ByDepartment_TotalsMatchOverallPayroll()
        {
            Assert.Equal(_payroll.TotalPayroll(), _payroll.ByDepartment().Sum(d => d.Total));
        }

        [Fact]
        public void EmptyPayroll_IsZero()
        {
            var empty = new PayrollService(Array.Empty<Employee>());

            Assert.Equal(0m, empty.TotalPayroll());
            Assert.Equal(0m, empty.TotalTax());
            Assert.Empty(empty.ByDepartment());
        }

        [Fact]
        public void NullEmployees_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PayrollService(null!));
        }

        [Fact]
        public void Employees_IsReadOnly()
        {
            Assert.Throws<NotSupportedException>(() => ((IList<Employee>)_payroll.Employees).Clear());
        }

        [Fact]
        public void Demo_Run_PrintsTotalsAndGrouping()
        {
            string output = ConsoleCapture.Run(PayrollDemo.Run);

            Assert.Contains("Total payroll: 309,000.00", output);
            Assert.Contains("Total tax (ITaxable only): 19,500.00", output);
            Assert.Contains("Engineering  2 employees, total 120,000.00", output);
            Assert.Contains("Marketing    2 employees, total 79,000.00", output);
        }
    }
}
