// Mini Q4 - Employee Payroll: abstract base, three employee types, ITaxable on full-time only.
namespace ExtensionsMiniProjects.MiniProjects.Payroll
{
    // Only some employees are taxed by the company payroll - an interface marks that capability.
    public interface ITaxable
    {
        decimal TaxRate { get; }
        decimal CalculateTax();
    }

    public abstract class Employee
    {
        public string Name { get; }
        public string Department { get; }

        protected Employee(string name, string department)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department is required.", nameof(department));
            Name = name;
            Department = department;
        }

        public abstract string EmployeeType { get; }

        // Each employee type calculates its own monthly gross salary.
        public abstract decimal CalculateSalary();

        public override string ToString() => $"{Name} ({EmployeeType}, {Department})";
    }

    public class FullTimeEmployee : Employee, ITaxable
    {
        public decimal MonthlySalary { get; }
        public decimal TaxRate { get; }

        public FullTimeEmployee(string name, string department, decimal monthlySalary, decimal taxRate = 0.10m)
            : base(name, department)
        {
            if (monthlySalary < 0)
                throw new ArgumentOutOfRangeException(nameof(monthlySalary), monthlySalary, "Salary cannot be negative.");
            if (taxRate < 0 || taxRate > 1)
                throw new ArgumentOutOfRangeException(nameof(taxRate), taxRate, "Tax rate must be between 0 and 1.");
            MonthlySalary = monthlySalary;
            TaxRate = taxRate;
        }

        public override string EmployeeType => "Full-time";

        public override decimal CalculateSalary() => MonthlySalary;

        public decimal CalculateTax() => Math.Round(MonthlySalary * TaxRate, 2);
    }

    public class PartTimeEmployee : Employee
    {
        public decimal HourlyRate { get; }
        public decimal HoursWorked { get; }

        public PartTimeEmployee(string name, string department, decimal hourlyRate, decimal hoursWorked)
            : base(name, department)
        {
            if (hourlyRate < 0)
                throw new ArgumentOutOfRangeException(nameof(hourlyRate), hourlyRate, "Hourly rate cannot be negative.");
            if (hoursWorked < 0)
                throw new ArgumentOutOfRangeException(nameof(hoursWorked), hoursWorked, "Hours cannot be negative.");
            HourlyRate = hourlyRate;
            HoursWorked = hoursWorked;
        }

        public override string EmployeeType => "Part-time";

        public override decimal CalculateSalary() => HourlyRate * HoursWorked;
    }

    public class ContractEmployee : Employee
    {
        public decimal ContractAmount { get; }
        public int ContractMonths { get; }

        public ContractEmployee(string name, string department, decimal contractAmount, int contractMonths)
            : base(name, department)
        {
            if (contractAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(contractAmount), contractAmount, "Contract amount cannot be negative.");
            if (contractMonths <= 0)
                throw new ArgumentOutOfRangeException(nameof(contractMonths), contractMonths, "Contract must last at least one month.");
            ContractAmount = contractAmount;
            ContractMonths = contractMonths;
        }

        public override string EmployeeType => "Contract";

        // The contract fee is spread evenly across its months.
        public override decimal CalculateSalary() => Math.Round(ContractAmount / ContractMonths, 2);
    }
}
