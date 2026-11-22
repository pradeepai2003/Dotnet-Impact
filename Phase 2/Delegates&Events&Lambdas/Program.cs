using DelegatesEventsLambdas.BuiltInDelegates;
using DelegatesEventsLambdas.Delegates;
using DelegatesEventsLambdas.Events;

namespace DelegatesEventsLambdas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 2.8: Delegates, multicast & Func ===");
            DelegateDemo.Run();

            Console.WriteLine("\n=== Task 2.9: Events (AlarmClock) ===");
            AlarmDemo.Run();

            Console.WriteLine("\n=== Task 2.10: Action / Func / Predicate ===");
            BuiltInDelegatesDemo.Run();
        }
    }
}
