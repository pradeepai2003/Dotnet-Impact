// Task 2.13 - LINQ: every query written twice (query syntax + method syntax).
using System.Globalization;

namespace GenericsYieldLinq.Linq
{
    public record Employee(string Name, string Department, decimal Salary, DateTime JoiningDate);

    public record DepartmentStat(string Department, int Count, decimal AverageSalary);

    public static class EmployeeData
    {
        public static List<Employee> Sample() => new()
        {
            new("Asha",    "Engineering", 95000m, new DateTime(2016, 3, 14)),
            new("Ravi",    "Engineering", 72000m, new DateTime(2019, 7, 1)),
            new("Meena",   "Engineering", 48000m, new DateTime(2023, 1, 9)),
            new("Karthik", "Finance",     67000m, new DateTime(2018, 11, 20)),
            new("Sneha",   "Finance",     45000m, new DateTime(2022, 5, 16)),
            new("John",    "Finance",     52000m, new DateTime(2020, 9, 7)),
            new("Lakshmi", "HR",          41000m, new DateTime(2021, 2, 1)),
            new("Arif",    "HR",          58000m, new DateTime(2015, 6, 30)),
            new("Divya",   "Marketing",   61000m, new DateTime(2017, 8, 21)),
            new("Vikram",  "Marketing",   39000m, new DateTime(2024, 4, 15)),
            new("Priya",   "Marketing",   50000m, new DateTime(2019, 12, 2)),
            new("Suresh",  "Engineering", 88000m, new DateTime(2014, 10, 5)),
        };
    }

    public static class EmployeeQueries
    {
        // ---- 1. Filter: salary > threshold ----
        public static List<Employee> HighEarnersQuery(IEnumerable<Employee> employees, decimal threshold = 50000m) =>
            (from e in employees
             where e.Salary > threshold
             select e).ToList();

        public static List<Employee> HighEarnersMethod(IEnumerable<Employee> employees, decimal threshold = 50000m) =>
            employees.Where(e => e.Salary > threshold).ToList();

        // ---- 2. Order by salary descending (name breaks ties so the order is stable) ----
        public static List<Employee> BySalaryDescQuery(IEnumerable<Employee> employees) =>
            (from e in employees
             orderby e.Salary descending, e.Name
             select e).ToList();

        public static List<Employee> BySalaryDescMethod(IEnumerable<Employee> employees) =>
            employees.OrderByDescending(e => e.Salary).ThenBy(e => e.Name).ToList();

        // ---- 3. Group by department: count + average salary ----
        public static List<DepartmentStat> DepartmentStatsQuery(IEnumerable<Employee> employees) =>
            (from e in employees
             group e by e.Department into g
             orderby g.Key
             select new DepartmentStat(g.Key, g.Count(), Math.Round(g.Average(x => x.Salary), 2))).ToList();

        public static List<DepartmentStat> DepartmentStatsMethod(IEnumerable<Employee> employees) =>
            employees.GroupBy(e => e.Department)
                     .OrderBy(g => g.Key)
                     .Select(g => new DepartmentStat(g.Key, g.Count(), Math.Round(g.Average(x => x.Salary), 2)))
                     .ToList();

        // ---- 4. Project to an anonymous type { Name, Experience } ----
        // The anonymous type is used INSIDE the method; it is converted to a tuple to be returned
        // (an anonymous type has no name, so it cannot appear in a method signature).
        public static List<(string Name, int Experience)> ExperienceQuery(IEnumerable<Employee> employees, DateTime asOf)
        {
            var anonymous = from e in employees
                            orderby e.Name
                            select new { e.Name, Experience = YearsBetween(e.JoiningDate, asOf) };
            return anonymous.Select(a => (a.Name, a.Experience)).ToList();
        }

        public static List<(string Name, int Experience)> ExperienceMethod(IEnumerable<Employee> employees, DateTime asOf)
        {
            var anonymous = employees.OrderBy(e => e.Name)
                                     .Select(e => new { e.Name, Experience = YearsBetween(e.JoiningDate, asOf) });
            return anonymous.Select(a => (a.Name, a.Experience)).ToList();
        }

        // Whole years completed between two dates (never negative).
        public static int YearsBetween(DateTime from, DateTime to)
        {
            int years = to.Year - from.Year;
            if (to < from.AddYears(years))
                years--;
            return Math.Max(0, years);
        }
    }

    public static class LinqDemo
    {
        public static readonly DateTime AsOf = new(2026, 11, 22);

        public static void Run()
        {
            List<Employee> employees = EmployeeData.Sample();

            var highQ = EmployeeQueries.HighEarnersQuery(employees);
            var highM = EmployeeQueries.HighEarnersMethod(employees);
            Console.WriteLine($"1) Salary > 50,000: {string.Join(", ", highQ.Select(e => e.Name))}");
            Console.WriteLine($"   query == method: {highQ.SequenceEqual(highM)}");

            var sortQ = EmployeeQueries.BySalaryDescQuery(employees);
            var sortM = EmployeeQueries.BySalaryDescMethod(employees);
            Console.WriteLine($"2) By salary desc: {string.Join(", ", sortQ.Select(e => $"{e.Name} {Fmt(e.Salary)}"))}");
            Console.WriteLine($"   query == method: {sortQ.SequenceEqual(sortM)}");

            var statQ = EmployeeQueries.DepartmentStatsQuery(employees);
            var statM = EmployeeQueries.DepartmentStatsMethod(employees);
            Console.WriteLine("3) By department:");
            foreach (DepartmentStat s in statQ)
                Console.WriteLine($"   {s.Department,-12} count {s.Count}, avg {Fmt(s.AverageSalary)}");
            Console.WriteLine($"   query == method: {statQ.SequenceEqual(statM)}");

            var expQ = EmployeeQueries.ExperienceQuery(employees, AsOf);
            var expM = EmployeeQueries.ExperienceMethod(employees, AsOf);
            Console.WriteLine($"4) Experience (as of {AsOf:yyyy-MM-dd}): {string.Join(", ", expQ.Select(x => $"{x.Name} {x.Experience}y"))}");
            Console.WriteLine($"   query == method: {expQ.SequenceEqual(expM)}");
        }

        private static string Fmt(decimal d) => d.ToString("N2", CultureInfo.InvariantCulture);
    }
}
