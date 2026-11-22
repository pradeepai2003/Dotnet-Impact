using GenericsYieldLinq.Generics;
using GenericsYieldLinq.Linq;
using GenericsYieldLinq.Yield;

namespace GenericsYieldLinq
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 2.11: Generic Repository<T> ===");
            RepositoryDemo.Run();

            Console.WriteLine("\n=== Task 2.12: yield return & IEnumerable<T> ===");
            IteratorDemo.Run();

            Console.WriteLine("\n=== Task 2.13: LINQ (query vs method syntax) ===");
            LinqDemo.Run();
        }
    }
}
