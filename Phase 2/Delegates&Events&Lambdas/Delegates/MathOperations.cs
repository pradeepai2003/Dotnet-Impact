// Task 2.8 - Custom delegate, multicast delegate, and the Func<> equivalent.
using System.Globalization;

namespace DelegatesEventsLambdas.Delegates
{
    // A delegate is a TYPE that describes a method signature: (double, double) -> double.
    public delegate double MathOperation(double a, double b);

    public static class MathOperations
    {
        public static double Add(double a, double b) => a + b;
        public static double Subtract(double a, double b) => a - b;
        public static double Multiply(double a, double b) => a * b;

        public static double Divide(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero.");
            return a / b;
        }

        // Invoking a multicast delegate directly returns ONLY the last method's result.
        // GetInvocationList() lets us run each method and keep every result.
        public static double[] InvokeAll(MathOperation operation, double a, double b)
        {
            ArgumentNullException.ThrowIfNull(operation);
            return operation.GetInvocationList()
                            .Cast<MathOperation>()
                            .Select(op => op(a, b))
                            .ToArray();
        }
    }

    public static class DelegateDemo
    {
        public static void Run()
        {
            const double x = 10, y = 5;

            // 1) Custom delegate pointing at each method.
            MathOperation[] operations = { MathOperations.Add, MathOperations.Subtract, MathOperations.Multiply, MathOperations.Divide };
            foreach (MathOperation op in operations)
            {
                Console.WriteLine($"{op.Method.Name,-8}({x}, {y}) = {Fmt(op(x, y))}");
            }

            // 2) Multicast: one delegate, two methods (+= adds to the invocation list).
            MathOperation multicast = MathOperations.Add;
            multicast += MathOperations.Multiply;
            Console.WriteLine($"Multicast methods: {string.Join(" + ", multicast.GetInvocationList().Select(d => d.Method.Name))}");
            Console.WriteLine($"Direct call returns last result only: {Fmt(multicast(x, y))}");
            Console.WriteLine($"Each method's result: {string.Join(", ", MathOperations.InvokeAll(multicast, x, y).Select(Fmt))}");

            // 3) Same operations rewritten with the built-in Func<double, double, double> -
            //    no custom delegate type needed. Lambdas replace the named methods.
            (string Name, Func<double, double, double> Func)[] funcs =
            {
                ("Add", AddFunc),
                ("Subtract", SubtractFunc),
                ("Multiply", MultiplyFunc),
                ("Divide", DivideFunc),
            };
            for (int i = 0; i < funcs.Length; i++)
            {
                double funcResult = funcs[i].Func(x, y);
                Console.WriteLine($"Func {funcs[i].Name,-8}= {Fmt(funcResult)} (matches delegate: {funcResult == operations[i](x, y)})");
            }

            // Func multicast works the same way.
            Func<double, double, double> multicastFunc = MathOperations.Add;
            multicastFunc += MathOperations.Multiply;
            Console.WriteLine($"Func multicast methods: {string.Join(" + ", multicastFunc.GetInvocationList().Select(d => d.Method.Name))}");
        }

        // The Func equivalents of the four delegate methods, exposed for reuse and testing.
        public static readonly Func<double, double, double> AddFunc = (a, b) => a + b;
        public static readonly Func<double, double, double> SubtractFunc = (a, b) => a - b;
        public static readonly Func<double, double, double> MultiplyFunc = (a, b) => a * b;
        public static readonly Func<double, double, double> DivideFunc =
            (a, b) => b == 0 ? throw new DivideByZeroException("Cannot divide by zero.") : a / b;

        private static string Fmt(double value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
