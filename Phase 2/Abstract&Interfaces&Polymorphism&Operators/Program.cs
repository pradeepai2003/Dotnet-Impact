using AbstractInterfacesPolymorphism.Abstract;
using AbstractInterfacesPolymorphism.Hiding;
using AbstractInterfacesPolymorphism.Interfaces;
using AbstractInterfacesPolymorphism.Operators;
using AbstractInterfacesPolymorphism.Polymorphism;

namespace AbstractInterfacesPolymorphism
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 2.4: Abstract class ===");
            ShapeDemo.Run();

            Console.WriteLine("\n=== Task 2.5: Multiple interfaces ===");
            InterfaceDemo.Run();

            Console.WriteLine("\n=== Task 2.6: Overloading & runtime polymorphism ===");
            PolymorphismDemo.Run();

            Console.WriteLine("\n=== Task 2.6: Method hiding (new) vs override ===");
            HidingDemo.Run();

            Console.WriteLine("\n=== Task 2.7: Operator overloading (Money) ===");
            MoneyDemo.Run();
        }
    }
}
