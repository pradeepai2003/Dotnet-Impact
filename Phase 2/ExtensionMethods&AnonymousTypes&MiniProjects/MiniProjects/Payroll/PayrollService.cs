// Mini Q4 - Payroll totals via polymorphism + LINQ GroupBy department.
using System.Globalization;

namespace ExtensionsMiniProjects.MiniProjects.Payroll
{
    public record DepartmentPayroll(string Department, int Headcount, decimal Total);

    public class PayrollService
    {
        private readonly List<Employee> _employees;

        public PayrollService(IEnumerable<Employee> employees)
        {
            ArgumentNullException.ThrowIfNull(employees);
            _employees = employees.ToList();
        }

        public IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        // POLYMORPHISM: one call, three different CalculateSalary() implementations.
        public decimal TotalPayroll() => _employees.Sum(e => e.CalculateSalary());

        // ITaxable path: only employees that implement the interface are taxed.
        public decimal TaxFor(Employee employee) => employee is ITaxable taxable ? taxable.CalculateTax() : 0m;

        public decimal TotalTax() => _employees.OfType<ITaxable>().Sum(t => t.CalculateTax());

        public decimal NetPayFor(Employee employee) => employee.CalculateSalary() - TaxFor(employee);

        public List<DepartmentPayroll> ByDepartment() =>
            _employees.GroupBy(e => e.Department)
                      .OrderBy(g => g.Key)
                      .Select(g => new DepartmentPayroll(g.Key, g.Count(), g.Sum(e => e.CalculateSalary())))
                      .ToList();

        public static PayrollService CreateSample() => new(new Employee[]
        {
            new FullTimeEmployee("Asha", "Engineering", 80000m),
            new PartTimeEmployee("Ravi", "Engineering", 500m, 80m),
            new FullTimeEmployee("Meena", "Finance", 60000m),
            new ContractEmployee("John", "Finance", 300000m, 6),
            new FullTimeEmployee("Karthik", "Marketing", 55000m),
            new PartTimeEmployee("Sneha", "Marketing", 400m, 60m),
        });
    }

    public static class PayrollDemo
    {
        public static void Run()
        {
            PayrollService payroll = PayrollService.CreateSample();

            Console.WriteLine($"{"Employee",-9} {"Type",-10} {"Gross",11} {"Tax",10} {"Net",11}");
            foreach (Employee e in payroll.Employees)
            {
                Console.WriteLine($"{e.Name,-9} {e.EmployeeType,-10} {Fmt(e.CalculateSalary()),11} {Fmt(payroll.TaxFor(e)),10} {Fmt(payroll.NetPayFor(e)),11}");
            }
            Console.WriteLine($"Total payroll: {Fmt(payroll.TotalPayroll())}");
            Console.WriteLine($"Total tax (ITaxable only): {Fmt(payroll.TotalTax())}");

            Console.WriteLine("By department:");
            foreach (DepartmentPayroll d in payroll.ByDepartment())
            {
                Console.WriteLine($"  {d.Department,-12} {d.Headcount} employees, total {Fmt(d.Total)}");
            }
        }

        private static string Fmt(decimal amount) => amount.ToString("N2", CultureInfo.InvariantCulture);
    }
}
