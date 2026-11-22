using EncapsulationInheritanceOverriding.Encapsulation;
using EncapsulationInheritanceOverriding.Inheritance;
using EncapsulationInheritanceOverriding.Overriding;

namespace EncapsulationInheritanceOverriding
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 2.1: Encapsulation (BankAccount) ===");
            BankAccountDemo.Run();

            Console.WriteLine("\n=== Task 2.2: Inheritance & constructor chaining ===");
            VehicleDemo.Run();

            Console.WriteLine("\n=== Task 2.3: virtual / override / sealed ===");
            NotificationDemo.Run();
        }
    }
}
