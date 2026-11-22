using System.Reflection;
using AbstractInterfacesPolymorphism.Abstract;
using AbstractInterfacesPolymorphism.Hiding;
using AbstractInterfacesPolymorphism.Operators;
using AbstractInterfacesPolymorphism.Polymorphism;

namespace AbstractInterfacesPolymorphism.Tests
{
    // Task 2.6 - overloading + runtime polymorphism
    public class CalculatorTests
    {
        [Fact]
        public void Add_IntInt() => Assert.Equal(5, Calculator.Add(2, 3));

        [Fact]
        public void Add_DoubleDouble() => Assert.Equal(5.6, Calculator.Add(2.5, 3.1), precision: 10);

        [Fact]
        public void Add_ThreeInts() => Assert.Equal(6, Calculator.Add(1, 2, 3));

        [Fact]
        public void Add_Params() => Assert.Equal(15, Calculator.Add(1, 2, 3, 4, 5));

        [Fact]
        public void Add_ParamsEmpty_IsZero() => Assert.Equal(0, Calculator.Add());

        [Fact]
        public void Add_ParamsNull_Throws() => Assert.Throws<ArgumentNullException>(() => Calculator.Add((int[])null!));

        [Fact]
        public void Add_IntIntOverload_ReturnsInt()
        {
            object result = Calculator.Add(2, 3);
            Assert.IsType<int>(result);
        }

        [Fact]
        public void ShapeList_RuntimePolymorphism_PicksDerivedArea()
        {
            List<Shape> shapes = new() { new Circle(1), new Rectangle(2, 3) };

            Assert.Equal(Math.PI, shapes[0].CalculateArea(), precision: 10);
            Assert.Equal(6, shapes[1].CalculateArea());
            Assert.Equal(Math.PI + 6, PolymorphismDemo.TotalArea(shapes), precision: 10);
        }

        [Fact]
        public void TotalArea_Empty_IsZero() => Assert.Equal(0, PolymorphismDemo.TotalArea(new List<Shape>()));

        [Fact]
        public void PolymorphismDemo_Run_PrintsOverloadsAndAreas()
        {
            string output = ConsoleCapture.Run(PolymorphismDemo.Run);

            Assert.Contains("Add(1, 2, 3, 4, 5) = 15", output);
            Assert.Contains("Circle    -> area 3.14", output);
            Assert.Contains("Rectangle -> area 6.00", output);
            Assert.Contains("Total area = 28.78", output);
        }
    }

    // Task 2.6 - new (hiding) vs override
    public class HidingTests
    {
        [Fact]
        public void New_ReferenceTypeDecides()
        {
            var fileLogger = new FileLogger();
            Logger asBase = fileLogger;

            Assert.Equal("[FileLogger -> app.log] saved", fileLogger.Log("saved"));
            Assert.Equal("[Logger] saved", asBase.Log("saved"));   // hidden: base version runs
        }

        [Fact]
        public void Override_ObjectTypeDecides()
        {
            var fileLogger = new FileLogger();
            Logger asBase = fileLogger;

            Assert.Equal("File logger writing to app.log", asBase.Describe());
            Assert.Equal("Base logger", new Logger().Describe());
        }

        [Fact]
        public void HidingMethod_IsNotVirtual()
        {
            MethodInfo log = typeof(FileLogger).GetMethod(nameof(FileLogger.Log), new[] { typeof(string) })!;

            Assert.Equal(typeof(FileLogger), log.DeclaringType);
            Assert.False(log.IsVirtual);
        }

        [Fact]
        public void HidingDemo_Run_DocumentsBothBehaviours()
        {
            string output = ConsoleCapture.Run(HidingDemo.Run);

            Assert.Contains("'new' hides: base version runs", output);
            Assert.Contains("'override': derived version runs", output);
        }
    }

    // Task 2.7 - operator overloading
    public class MoneyTests
    {
        private static readonly Money Inr100 = new(100m, "INR");

        [Fact]
        public void Constructor_NormalisesCurrency()
        {
            var money = new Money(5m, " inr ");

            Assert.Equal("INR", money.Currency);
            Assert.Equal(5m, money.Amount);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Constructor_BlankCurrency_Throws(string currency)
        {
            Assert.Throws<ArgumentException>(() => new Money(1m, currency));
        }

        [Fact]
        public void Plus_SameCurrency_AddsAmounts()
        {
            Money total = Inr100 + new Money(250.50m, "INR");

            Assert.Equal(new Money(350.50m, "INR"), total);
        }

        [Fact]
        public void Plus_MismatchedCurrency_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Inr100 + new Money(10m, "USD"));
            Assert.Equal("Cannot add INR and USD.", ex.Message);
        }

        [Fact]
        public void Equality_SameAmountAndCurrency()
        {
            Assert.True(Inr100 == new Money(100m, "INR"));
            Assert.False(Inr100 != new Money(100m, "INR"));
        }

        [Fact]
        public void Equality_DifferentAmountOrCurrency()
        {
            Assert.False(Inr100 == new Money(99m, "INR"));
            Assert.True(Inr100 != new Money(100m, "USD"));
        }

        [Fact]
        public void Comparison_SameCurrency()
        {
            var bigger = new Money(200m, "INR");

            Assert.True(bigger > Inr100);
            Assert.False(Inr100 > bigger);
            Assert.True(Inr100 < bigger);
            Assert.False(bigger < Inr100);
        }

        [Fact]
        public void Comparison_MismatchedCurrency_Throws()
        {
            var usd = new Money(1m, "USD");

            Assert.Throws<InvalidOperationException>(() => Inr100 > usd);
            Assert.Throws<InvalidOperationException>(() => Inr100 < usd);
        }

        [Fact]
        public void Equals_Object_And_HashCode()
        {
            Assert.True(Inr100.Equals((object)new Money(100m, "INR")));
            Assert.False(Inr100.Equals("100 INR"));
            Assert.Equal(Inr100.GetHashCode(), new Money(100m, "INR").GetHashCode());
        }

        [Fact]
        public void ToString_FormatsAmountAndCurrency()
        {
            Assert.Equal("1,234.50 INR", new Money(1234.5m, "INR").ToString());
        }

        [Fact]
        public void MoneyDemo_Run_ShowsResultsAndMismatchError()
        {
            string output = ConsoleCapture.Run(MoneyDemo.Run);

            Assert.Contains("100.00 INR + 250.50 INR = 350.50 INR", output);
            Assert.Contains("100.00 INR > 250.50 INR ? False", output);
            Assert.Contains("InvalidOperationException: Cannot add INR and USD.", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => AbstractInterfacesPolymorphism.Program.Main(Array.Empty<string>()));

            string[] headers = { "=== Task 2.4", "=== Task 2.5", "=== Task 2.6: Overloading", "=== Task 2.6: Method hiding", "=== Task 2.7" };
            int position = -1;
            foreach (string header in headers)
            {
                int next = output.IndexOf(header, StringComparison.Ordinal);
                Assert.True(next > position, $"Expected \"{header}\" after position {position}.");
                position = next;
            }
        }
    }
}
