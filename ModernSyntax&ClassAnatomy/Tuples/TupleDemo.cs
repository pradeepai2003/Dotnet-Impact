// Task 1.14 - Named tuples and deconstruction.
namespace ModernSyntaxClassAnatomy.Tuples
{
    public static class TupleDemo
    {
        public static void Run()
        {
            // ---- Named tuple returned from a method ----
            int[] scores = { 72, 95, 48, 88, 63 };
            (int Min, int Max) result = GetMinMax(scores);
            Console.WriteLine($"Using tuple names: Min = {result.Min}, Max = {result.Max}");

            // Deconstruct into two separate variables
            var (min, max) = GetMinMax(scores);
            Console.WriteLine($"Deconstructed:     min = {min}, max = {max}");

            // ---- Employee lookup consumed by deconstruction ----
            var (name, age, department) = GetEmployee(102);
            Console.WriteLine($"Employee 102: {name}, {age}, {department}");

            // Discard (_) the parts you don't need
            var (onlyName, _, onlyDept) = GetEmployee(101);
            Console.WriteLine($"Employee 101: {onlyName} works in {onlyDept}");
        }

        public static (int Min, int Max) GetMinMax(int[] numbers)
        {
            int min = numbers[0];
            int max = numbers[0];
            foreach (int n in numbers)
            {
                if (n < min) min = n;
                if (n > max) max = n;
            }
            return (min, max);
        }

        public static (string Name, int Age, string Department) GetEmployee(int id)
        {
            return id switch
            {
                101 => ("Anita", 29, "Finance"),
                102 => ("Karthik", 35, "Engineering"),
                _   => ("Unknown", 0, "N/A")
            };
        }
    }
}
