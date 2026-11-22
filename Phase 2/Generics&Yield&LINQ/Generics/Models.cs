// Task 2.11 - Two unrelated types stored in the SAME generic repository.
using System.Globalization;

namespace GenericsYieldLinq.Generics
{
    public class Student : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Grade { get; set; }

        public override string ToString() => $"Student #{Id} {Name} (grade {Grade})";
    }

    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }

        public override string ToString() => $"Product #{Id} {Name} ({Price.ToString("N2", CultureInfo.InvariantCulture)})";
    }

    public static class RepositoryDemo
    {
        public static void Run()
        {
            var students = new Repository<Student>();
            students.Add(new Student { Id = 1, Name = "Asha", Grade = 8 });
            students.Add(new Student { Id = 2, Name = "Ravi", Grade = 9 });
            students.Update(new Student { Id = 2, Name = "Ravi Kumar", Grade = 10 });
            Student created = students.CreateNew(3);
            created.Name = "Meena";
            created.Grade = 9;
            students.Add(created);
            students.Delete(1);
            Print("Students", students.GetAll());

            var products = new Repository<Product>();
            products.Add(new Product { Id = 10, Name = "Laptop", Price = 65000m });
            products.Add(new Product { Id = 11, Name = "Mouse", Price = 799m });
            Console.WriteLine($"Update missing Id 99 -> {products.Update(new Product { Id = 99 })}");
            Print("Products", products.GetAll());

            try
            {
                products.Add(new Product { Id = 10, Name = "Duplicate" });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Duplicate add rejected: {ex.Message}");
            }

            // var numbers = new Repository<int>();
            //   ERROR CS0452: The type 'int' must be a reference type in order to use it as parameter 'T'
        }

        private static void Print<T>(string title, IEnumerable<T> items)
        {
            Console.WriteLine($"{title}:");
            foreach (T item in items)
                Console.WriteLine($"  {item}");
        }
    }
}
