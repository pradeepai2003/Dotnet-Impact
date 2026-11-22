// Task 2.6 (part 1) - Compile-time polymorphism (overloading) and runtime polymorphism (overriding).
using System.Globalization;
using AbstractInterfacesPolymorphism.Abstract;

namespace AbstractInterfacesPolymorphism.Polymorphism
{
    // COMPILE-TIME polymorphism: same name, different parameter lists.
    // The compiler picks the overload from the arguments.
    public static class Calculator
    {
        public static int Add(int a, int b) => a + b;

        public static double Add(double a, double b) => a + b;

        public static int Add(int a, int b, int c) => a + b + c;

        // params: any number of ints (used only when no fixed overload matches better).
        public static int Add(params int[] numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);
            return numbers.Sum();
        }
    }

    public static class PolymorphismDemo
    {
        // RUNTIME polymorphism: the variable type is Shape, but the OBJECT's type
        // decides which CalculateArea() runs.
        public static double TotalArea(IEnumerable<Shape> shapes) => shapes.Sum(s => s.CalculateArea());

        public static void Run()
        {
            Console.WriteLine($"Add(2, 3)          = {Calculator.Add(2, 3)}         (int, int)");
            Console.WriteLine($"Add(2.5, 3.1)      = {Calculator.Add(2.5, 3.1).ToString(CultureInfo.InvariantCulture)}       (double, double)");
            Console.WriteLine($"Add(1, 2, 3)       = {Calculator.Add(1, 2, 3)}         (int, int, int)");
            Console.WriteLine($"Add(1, 2, 3, 4, 5) = {Calculator.Add(1, 2, 3, 4, 5)}        (params int[])");

            List<Shape> shapes = new() { new Circle(1), new Rectangle(2, 3), new Circle(2.5) };
            foreach (Shape shape in shapes)
            {
                Console.WriteLine($"{shape.GetType().Name,-9} -> area {shape.CalculateArea().ToString("F2", CultureInfo.InvariantCulture)}");
            }
            Console.WriteLine($"Total area = {TotalArea(shapes).ToString("F2", CultureInfo.InvariantCulture)}");
        }
    }
}
