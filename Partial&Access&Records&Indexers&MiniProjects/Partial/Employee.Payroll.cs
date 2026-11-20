// Task 1.17 - Partial class, file 2 of 2: behaviour + partial method implementation.
namespace PartialAccessRecordsIndexers.Partial
{
    public partial class Employee
    {
        // Uses Id/Name/Salary defined in the OTHER file - it's all one class.
        public string GetPayslip() => $"Payslip: #{Id} {Name} - {Salary:N2} per month";

        // Partial method IMPLEMENTATION (has the body).
        partial void OnSalaryChanged(decimal oldSalary, decimal newSalary)
        {
            Console.WriteLine($"  [audit] {Name}'s salary changed {oldSalary:N2} -> {newSalary:N2}");
        }
    }

    public static class PartialDemo
    {
        public static void Run()
        {
            var emp = new Employee(7, "Deepa", 50000);
            Console.WriteLine(emp.GetPayslip());
            emp.GiveRaise(10);
            Console.WriteLine(emp.GetPayslip());
        }
    }
}
