// Task 2.4 - Abstract class with an abstract method and a concrete method.
using System.Globalization;

namespace AbstractInterfacesPolymorphism.Abstract
{
    // 'abstract': a blueprint only - it cannot be instantiated on its own.
    public abstract class Shape
    {
        public string Name { get; }

        protected Shape(string name) => Name = name;

        // Abstract: NO body here; every concrete subclass MUST provide one.
        public abstract double CalculateArea();

        // Concrete: shared behaviour every shape inherits as-is.
        public void DisplayArea() =>
            Console.WriteLine($"{Name} area: {CalculateArea().ToString("F2", CultureInfo.InvariantCulture)}");
    }

    public class Circle : Shape
    {
        public double Radius { get; }

        public Circle(double radius) : base("Circle")
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "Radius must be positive.");
            Radius = radius;
        }

        public override double CalculateArea() => Math.PI * Radius * Radius;
    }

    public class Rectangle : Shape
    {
        public double Width { get; }
        public double Height { get; }

        public Rectangle(double width, double height) : base("Rectangle")
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width and height must be positive.");
            Width = width;
            Height = height;
        }

        public override double CalculateArea() => Width * Height;
    }

    public static class ShapeDemo
    {
        public static void Run()
        {
            new Circle(2).DisplayArea();        // 12.57
            new Rectangle(4, 5).DisplayArea();  // 20.00

            // Shape shape = new Shape("Blob");
            //   ERROR CS0144: Cannot create an instance of the abstract type or interface 'Shape'
            //   WHY: an abstract class is incomplete (CalculateArea has no body), so only
            //        concrete subclasses such as Circle and Rectangle can be created.
        }
    }
}
