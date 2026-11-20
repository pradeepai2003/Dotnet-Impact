// Task 1.15 - Pattern matching: type, relational and property patterns.
namespace ModernSyntaxClassAnatomy.PatternMatching
{
    public enum OrderStatus { New, Shipped, Delivered, Cancelled }

    public class Order
    {
        public OrderStatus Status { get; set; }
        public decimal Amount { get; set; }
    }

    public static class PatternDemo
    {
        public static void Run()
        {
            // ---- Type patterns on an object parameter ----
            Console.WriteLine(Describe(42));
            Console.WriteLine(Describe("pattern"));
            Console.WriteLine(Describe(3.14));
            Console.WriteLine(Describe(null));
            Console.WriteLine(Describe(true));     // falls to the default branch

            // ---- Relational patterns in a switch expression ----
            Console.WriteLine();
            foreach (int marks in new[] { 95, 82, 71, 55, 30, 120 })
            {
                Console.WriteLine($"Marks {marks,3} -> Grade {GetGrade(marks)}");
            }

            // ---- Property patterns ----
            Console.WriteLine();
            var orders = new[]
            {
                new Order { Status = OrderStatus.New,       Amount = 6000 },
                new Order { Status = OrderStatus.New,       Amount = 1500 },
                new Order { Status = OrderStatus.Shipped,   Amount = 3000 },
                new Order { Status = OrderStatus.Cancelled, Amount = 9000 },
            };
            foreach (Order order in orders)
            {
                Console.WriteLine($"{order.Status,-9} {order.Amount,5} -> discount {GetDiscount(order):P0}");
            }
        }

        // Type pattern: checks the runtime type AND gives a typed variable in one step.
        public static string Describe(object? value) => value switch
        {
            null            => "null: no value supplied",
            int i           => $"int: {i} (doubled = {i * 2})",
            string s        => $"string: \"{s}\" (length {s.Length})",
            double d        => $"double: {d} (rounded = {Math.Round(d)})",
            _               => $"other type: {value.GetType().Name}"
        };

        // Relational patterns (<, >=, and) - checked top to bottom, first match wins.
        public static string GetGrade(int marks) => marks switch
        {
            < 0 or > 100 => "Invalid",
            >= 90        => "A",
            >= 80        => "B",
            >= 70        => "C",
            >= 50        => "D",
            _            => "F"
        };

        // Property patterns: match on the object's property values.
        public static decimal GetDiscount(Order order) => order switch
        {
            { Status: OrderStatus.Cancelled }                => 0m,     // never discount
            { Status: OrderStatus.New, Amount: >= 5000 }     => 0.15m,  // big new order
            { Status: OrderStatus.New }                      => 0.05m,  // any other new order
            { Amount: > 2000 }                               => 0.02m,
            _                                                => 0m
        };
    }
}
