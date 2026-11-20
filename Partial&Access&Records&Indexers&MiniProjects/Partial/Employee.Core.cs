// Task 1.17 - Partial class, file 1 of 2: data + constructor.
// 'partial' lets one class be split across several files. The compiler merges
// them into a single Employee class. Every part must use 'partial' and the same namespace.
namespace PartialAccessRecordsIndexers.Partial
{
    public partial class Employee
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Salary { get; private set; }

        public Employee(int id, string name, decimal salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }

        public void GiveRaise(decimal percent)
        {
            decimal oldSalary = Salary;
            Salary += Salary * percent / 100;
            OnSalaryChanged(oldSalary, Salary);   // calls the partial method
        }

        // Partial method DECLARATION (no body).
        // It is implemented in Employee.Payroll.cs. If no file implemented it, the
        // compiler would silently remove this declaration AND the call above.
        // (Rules for this classic form: returns void, implicitly private, no out params.)
        partial void OnSalaryChanged(decimal oldSalary, decimal newSalary);
    }
}
