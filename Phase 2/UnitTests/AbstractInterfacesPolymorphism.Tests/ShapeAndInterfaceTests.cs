using AbstractInterfacesPolymorphism.Abstract;
using AbstractInterfacesPolymorphism.Interfaces;

namespace AbstractInterfacesPolymorphism.Tests
{
    // Task 2.4 - abstract class
    public class ShapeTests
    {
        [Theory]
        [InlineData(1, Math.PI)]
        [InlineData(2, 4 * Math.PI)]
        public void Circle_CalculateArea(double radius, double expected)
        {
            Assert.Equal(expected, new Circle(radius).CalculateArea(), precision: 10);
        }

        [Fact]
        public void Rectangle_CalculateArea()
        {
            Assert.Equal(20, new Rectangle(4, 5).CalculateArea());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Circle_NonPositiveRadius_Throws(double radius)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(radius));
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(5, 0)]
        [InlineData(-1, 5)]
        public void Rectangle_NonPositiveSide_Throws(double width, double height)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Rectangle(width, height));
        }

        [Fact]
        public void DisplayArea_ConcreteMethod_UsesSubclassArea()
        {
            Assert.Equal("Circle area: 12.57", ConsoleCapture.Run(new Circle(2).DisplayArea).Trim());
            Assert.Equal("Rectangle area: 20.00", ConsoleCapture.Run(new Rectangle(4, 5).DisplayArea).Trim());
        }

        [Fact]
        public void Shape_IsAbstract_AndCannotBeInstantiated()
        {
            Assert.True(typeof(Shape).IsAbstract);
            Assert.True(typeof(Shape).GetMethod(nameof(Shape.CalculateArea))!.IsAbstract);
            Assert.ThrowsAny<MemberAccessException>(() => Activator.CreateInstance(typeof(Shape), nonPublic: true));
        }

        [Fact]
        public void ShapeDemo_Run_PrintsBothAreas()
        {
            string output = ConsoleCapture.Run(ShapeDemo.Run);

            Assert.Contains("Circle area: 12.57", output);
            Assert.Contains("Rectangle area: 20.00", output);
        }
    }

    // Task 2.5 - one class, two interfaces
    public class SquareTests
    {
        [Fact]
        public void Square_SatisfiesBothInterfaces()
        {
            var square = new Square(3);

            Assert.IsAssignableFrom<IShape>(square);
            Assert.IsAssignableFrom<IDrawable>(square);
        }

        [Fact]
        public void AreaAndPerimeter_ThroughIShape()
        {
            IShape shape = new Square(4);

            Assert.Equal(16, shape.CalculateArea());
            Assert.Equal(16, shape.CalculatePerimeter());
        }

        [Theory]
        [InlineData(1, "*")]
        [InlineData(2, "* *\n* *")]
        [InlineData(3, "* * *\n* * *\n* * *")]
        public void Draw_ThroughIDrawable(int side, string expected)
        {
            IDrawable drawable = new Square(side);

            Assert.Equal(expected, drawable.Draw());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void NonPositiveSide_Throws(int side)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Square(side));
        }

        [Fact]
        public void InterfaceDemo_Run_PrintsAreaPerimeterAndDrawing()
        {
            string output = ConsoleCapture.Run(InterfaceDemo.Run);

            Assert.Contains("Square(3) area      = 9", output);
            Assert.Contains("Square(3) perimeter = 12", output);
            Assert.Contains("* * *", output);
        }
    }
}
