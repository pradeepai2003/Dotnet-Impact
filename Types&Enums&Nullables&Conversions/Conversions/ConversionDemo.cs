// Task 1.12 - Implicit/explicit conversions and converting strings.
namespace TypesEnumsNullablesConversions.Conversions
{
    public static class ConversionDemo
    {
        public static void Run()
        {
            // ---- Implicit conversions (widening, safe, no cast needed) ----
            int i = 123456;
            long l = i;      // int -> long
            float f = l;     // long -> float
            double d = f;    // float -> double
            Console.WriteLine($"Implicit: int {i} -> long {l} -> float {f} -> double {d}");

            // ---- Explicit conversion (narrowing, needs a cast) ----
            double price = 99.99;
            int wholePrice = (int)price;   // cast required, otherwise CS0266 compile error
            Console.WriteLine($"Explicit: double {price} -> int {wholePrice}");
            // PRECISION LOSS: the cast TRUNCATES the decimal part (.99 is thrown away, not rounded).
            // The compiler forces an explicit cast because data can be lost.

            // ---- Converting a string: is, as, Convert.ToInt32, int.TryParse ----
            Console.WriteLine();
            object value = "123";   // a string stored in an object variable

            // 1) is  -> checks the TYPE of an object (does NOT parse text).
            //    Right when: you have an object and need to know what type it really is.
            Console.WriteLine($"is:   value is string? {value is string}");  // True
            Console.WriteLine($"is:   value is int?    {value is int}");     // False - "123" is text, not an int
            if (value is string text)                                         // pattern: check + cast in one step
            {
                Console.WriteLine($"is:   pattern matched string \"{text}\"");
            }

            // 2) as  -> casts to a reference/nullable type, returns null instead of throwing.
            //    Right when: the cast might fail and you'd rather check for null than catch an exception.
            string? asString = value as string;
            int? asInt = value as int?;
            Console.WriteLine($"as:   as string = \"{asString}\", as int? = {(asInt?.ToString() ?? "null")}");

            // 3) Convert.ToInt32 -> parses text into an int; THROWS FormatException on bad input.
            //    Right when: the input is known to be valid (e.g. from config); null becomes 0.
            Console.WriteLine($"Convert.ToInt32(\"123\") = {Convert.ToInt32("123")}");
            Console.WriteLine($"Convert.ToInt32(null)  = {Convert.ToInt32(null)}");
            try
            {
                Convert.ToInt32("abc");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Convert.ToInt32(\"abc\") threw {ex.GetType().Name}");
            }

            // 4) int.TryParse -> parses text, returns true/false, NEVER throws.
            //    Right when: the input is untrusted (user input, files) - the safest choice.
            foreach (string input in new[] { "456", "abc", "" })
            {
                if (int.TryParse(input, out int number))
                    Console.WriteLine($"int.TryParse(\"{input}\") succeeded: {number}");
                else
                    Console.WriteLine($"int.TryParse(\"{input}\") failed, no exception");
            }
        }
    }
}
