// Task 2.14 - Anonymous types: project Employee to { Name, AnnualSalary }, and why it can't be returned.
using System.Globalization;
using ExtensionsMiniProjects.MiniProjects.Payroll;

namespace ExtensionsMiniProjects.AnonymousTypes
{
    public static class AnonymousTypeDemo
    {
        public static void Run()
        {
            IReadOnlyList<Employee> employees = PayrollService.CreateSample().Employees;

            // Anonymous type: the compiler invents a read-only class with Name and AnnualSalary.
            // Each Employee type calculates its own monthly salary (polymorphism), x 12 for the year.
            var annual = employees.Select(e => new { e.Name, AnnualSalary = e.CalculateSalary() * 12 }).ToList();

            foreach (var row in annual)
            {
                Console.WriteLine($"{row.Name,-8} annual salary {row.AnnualSalary.ToString("N2", CultureInfo.InvariantCulture)}");
            }
            Console.WriteLine($"Compiler-generated type name: {annual[0].GetType().Name}");
        }

        /*
         * WHY an anonymous type can't be returned from a method:
         *   Its type has no name you can write in code (the compiler calls it something like
         *   <>f__AnonymousType0`2). A method signature needs a type name, so the only options
         *   are 'object' or 'dynamic' - both throw away compile-time checking, and callers can
         *   no longer write row.Name safely.
         *   FIX: project to a named type instead - a record, a class, or a tuple, as below.
         */
        public static List<(string Name, decimal AnnualSalary)> GetAnnualSalaries(IEnumerable<Employee> employees)
        {
            ArgumentNullException.ThrowIfNull(employees);
            return employees.Select(e => new { e.Name, AnnualSalary = e.CalculateSalary() * 12 })   // anonymous inside...
                            .Select(a => (a.Name, a.AnnualSalary))                                  // ...tuple outside
                            .ToList();
        }
    }
}
