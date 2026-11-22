// Task 2.10 - Built-in delegates Action<T>, Func<T,TResult>, Predicate<T>.
namespace DelegatesEventsLambdas.BuiltInDelegates
{
    public static class StandardDelegates
    {
        // Action<T>     : takes T, returns nothing (void).
        public static readonly Action<string> PrintUpper = text => Console.WriteLine(text.ToUpperInvariant());

        // Func<T1,T2,R> : takes inputs, RETURNS a value (last type argument is the return type).
        public static readonly Func<int, int, int> Multiply = (a, b) => a * b;

        // Predicate<T>  : takes T, returns bool - a yes/no test.
        public static readonly Predicate<int> IsEven = n => n % 2 == 0;
    }

    public static class ListProcessor
    {
        // filter -> transform -> output, each step supplied by the caller as a delegate.
        public static List<int> ProcessList(IEnumerable<int> list, Predicate<int> filter, Func<int, int> transform, Action<int> output)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(transform);
            ArgumentNullException.ThrowIfNull(output);

            var results = new List<int>();
            foreach (int item in list)
            {
                if (!filter(item))
                    continue;

                int transformed = transform(item);
                output(transformed);
                results.Add(transformed);
            }
            return results;
        }
    }

    public static class BuiltInDelegatesDemo
    {
        public static void Run()
        {
            StandardDelegates.PrintUpper("hello from action<string>");
            Console.WriteLine($"Multiply(6, 7) = {StandardDelegates.Multiply(6, 7)}");
            Console.WriteLine($"IsEven(4) = {StandardDelegates.IsEven(4)}, IsEven(7) = {StandardDelegates.IsEven(7)}");

            Console.WriteLine("ProcessList(1..10): filter evens -> square -> print");
            List<int> squares = ListProcessor.ProcessList(
                Enumerable.Range(1, 10),
                StandardDelegates.IsEven,
                n => StandardDelegates.Multiply(n, n),
                n => Console.WriteLine($"  {n}"));
            Console.WriteLine($"Result: [{string.Join(", ", squares)}]");   // [4, 16, 36, 64, 100]
        }
    }
}
