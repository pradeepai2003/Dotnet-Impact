// Task 1.17 - Records: value equality and non-destructive copy with 'with'.
namespace PartialAccessRecordsIndexers.Records
{
    // Positional record: the compiler generates the constructor, init-only properties,
    // value-based Equals/==, GetHashCode, ToString and Deconstruct.
    public record Address(string Street, string City, string Pincode);

    // A normal class with the same shape, for comparison.
    public class AddressClass
    {
        public string Street { get; init; } = "";
        public string City { get; init; } = "";
        public string Pincode { get; init; } = "";
    }

    public static class RecordDemo
    {
        public static void Run()
        {
            var home1 = new Address("12 MG Road", "Bengaluru", "560001");
            var home2 = new Address("12 MG Road", "Bengaluru", "560001");

            Console.WriteLine($"home1 = {home1}");                          // auto-generated ToString
            Console.WriteLine($"record: home1 == home2 ? {home1 == home2}"); // True  - same VALUES
            Console.WriteLine($"record: same object?    {ReferenceEquals(home1, home2)}"); // False - two objects

            // Class comparison: == compares REFERENCES, so equal data still gives False.
            var c1 = new AddressClass { Street = "12 MG Road", City = "Bengaluru", Pincode = "560001" };
            var c2 = new AddressClass { Street = "12 MG Road", City = "Bengaluru", Pincode = "560001" };
            Console.WriteLine($"class:  c1 == c2 ? {c1 == c2}");             // False

            // 'with' makes a COPY with some properties changed; the original is untouched.
            var office = home1 with { Street = "45 Brigade Road", Pincode = "560025" };
            Console.WriteLine($"with copy = {office}");
            Console.WriteLine($"original  = {home1}");
            Console.WriteLine($"home1 == office ? {home1 == office}");      // False - values differ

            // home1.City = "Chennai";
            //   ERROR CS8852: Init-only property can only be assigned in an object initializer...
            //   Records are immutable by default - use 'with' to get a modified copy.
        }
    }
}
