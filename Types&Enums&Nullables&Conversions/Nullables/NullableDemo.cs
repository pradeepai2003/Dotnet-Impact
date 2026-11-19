// Task 1.11 - Nullable value types and the ?? operator.
namespace TypesEnumsNullablesConversions.Nullables
{
    public static class NullableDemo
    {
        private const double DefaultDiscount = 0.05; // 5%

        public static void Run()
        {
            // ---- Nullable int with HasValue ----
            // A plain int can never be null. int? (Nullable<int>) can be a number OR null.
            int? marks = null;
            Console.WriteLine($"marks.HasValue = {marks.HasValue}");          // False

            marks = 85;
            if (marks.HasValue)
            {
                Console.WriteLine($"marks.HasValue = {marks.HasValue}, Value = {marks.Value}"); // True, 85
            }

            // ---- ApplyDiscount with default via ?? ----
            double price = 1000;
            Console.WriteLine($"Discount null -> {ApplyDiscount(price, null)}");   // 950 (default 5%)
            Console.WriteLine($"Discount 0.20 -> {ApplyDiscount(price, 0.20)}");   // 800 (uses 20%)
        }

        public static double ApplyDiscount(double price, double? discount)
        {
            // ?? returns the left side if it is not null, otherwise the right side.
            double rate = discount ?? DefaultDiscount;
            Console.WriteLine($"  Applying {rate:P0} discount on {price}");
            return price - (price * rate);
        }
    }
}
