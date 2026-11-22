// Task 2.5 - One class implementing two interfaces.
using System.Text;

namespace AbstractInterfacesPolymorphism.Interfaces
{
    public interface IShape
    {
        double CalculateArea();
        double CalculatePerimeter();
    }

    public interface IDrawable
    {
        string Draw();
    }

    // A class can inherit from only ONE base class, but can implement MANY interfaces.
    public class Square : IShape, IDrawable
    {
        public int Side { get; }

        public Square(int side)
        {
            if (side <= 0)
                throw new ArgumentOutOfRangeException(nameof(side), side, "Side must be positive.");
            Side = side;
        }

        public double CalculateArea() => Side * Side;

        public double CalculatePerimeter() => 4 * Side;

        public string Draw()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < Side; row++)
            {
                sb.Append(string.Join(" ", Enumerable.Repeat("*", Side)));
                if (row < Side - 1) sb.Append('\n');
            }
            return sb.ToString();
        }
    }

    /*
     * INTERFACE vs ABSTRACT CLASS
     * ---------------------------------------------------------------------------
     *                       | Interface                  | Abstract class
     * ---------------------------------------------------------------------------
     *  Purpose              | A CONTRACT: "can do X"     | A BASE: "is a kind of X"
     *  Multiple inheritance | Yes - implement many       | No - inherit only one
     *  Fields / state       | No instance fields         | Yes - fields, constructors
     *  Constructors         | No                         | Yes (protected, for subclasses)
     *  Implementation       | Members usually have none  | Mix of abstract + concrete
     *  Access modifiers     | Members public by default  | Any (private, protected...)
     * ---------------------------------------------------------------------------
     *  Use an INTERFACE when unrelated classes must offer the same capability
     *  (Square, a Chart and a Button can all be IDrawable).
     *  Use an ABSTRACT CLASS when related classes share state or code
     *  (every Shape has a Name and the same DisplayArea()).
     */

    public static class InterfaceDemo
    {
        public static void Run()
        {
            var square = new Square(3);

            IShape asShape = square;        // same object seen through each interface
            IDrawable asDrawable = square;

            Console.WriteLine($"Square(3) area      = {asShape.CalculateArea()}");
            Console.WriteLine($"Square(3) perimeter = {asShape.CalculatePerimeter()}");
            Console.WriteLine("Square(3) drawing:");
            Console.WriteLine(asDrawable.Draw());
        }
    }
}
