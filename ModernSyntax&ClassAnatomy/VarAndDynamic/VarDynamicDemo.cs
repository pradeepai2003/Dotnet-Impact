// Task 1.13 - var vs dynamic.
namespace ModernSyntaxClassAnatomy.VarAndDynamic
{
    public static class VarDynamicDemo
    {
        public static void Run()
        {
            // ---- var: type is fixed at COMPILE time ----
            var count = 10;          // compiler infers 'int'
            count = 20;              // OK - still an int
            // count = "ten";
            //   ERROR CS0029: Cannot implicitly convert type 'string' to 'int'
            //   WHY: 'var' is NOT a loosely-typed variable. The compiler decides the type
            //        from the first value (int) and it can never change afterwards.
            Console.WriteLine($"var count = {count}, type = {count.GetType().Name}");

            // ---- dynamic: type is checked at RUN time ----
            dynamic value = "Hello";
            Console.WriteLine($"dynamic value = {value}, type = {value.GetType().Name}");   // String

            value = 42;
            Console.WriteLine($"dynamic value = {value}, type = {value.GetType().Name}");   // Int32

            value = true;
            Console.WriteLine($"dynamic value = {value}, type = {value.GetType().Name}");   // Boolean
            // WHY: 'dynamic' skips compile-time type checking, so the same variable can hold
            //      any type. Mistakes only show up at runtime (RuntimeBinderException).

            // ---- dynamic parameters ----
            Console.WriteLine($"Add(5, 7)              = {Add(5, 7)}");                // 12
            Console.WriteLine($"Add(\"Hello, \", \"World\") = {Add("Hello, ", "World")}"); // Hello, World
            // WHY: '+' is resolved at runtime based on the actual types:
            //      int + int -> arithmetic addition, string + string -> concatenation.
        }

        public static dynamic Add(dynamic a, dynamic b)
        {
            return a + b;
        }
    }
}
