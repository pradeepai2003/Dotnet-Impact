using Microsoft.CSharp.RuntimeBinder;
using ModernSyntaxClassAnatomy.Tuples;
using ModernSyntaxClassAnatomy.VarAndDynamic;

namespace ModernSyntaxClassAnatomy.Tests
{
    // Task 1.13 - var vs dynamic
    public class VarDynamicTests
    {
        [Fact]
        public void Add_Ints_ReturnsSum()
        {
            dynamic result = VarDynamicDemo.Add(5, 7);

            Assert.Equal(12, (int)result);
            Assert.IsType<int>(result);
        }

        [Fact]
        public void Add_Strings_ReturnsConcatenation()
        {
            dynamic result = VarDynamicDemo.Add("Hello, ", "World");

            Assert.Equal("Hello, World", (string)result);
        }

        [Fact]
        public void Add_MixedIntAndDouble_ReturnsDouble()
        {
            dynamic result = VarDynamicDemo.Add(1.5, 2);

            Assert.Equal(3.5, (double)result);
        }

        [Fact]
        public void Add_UnsupportedTypes_FailsAtRuntime()
        {
            // No compile error - 'dynamic' defers the check to runtime.
            Assert.Throws<RuntimeBinderException>(() => VarDynamicDemo.Add(true, 1));
        }

        [Fact]
        public void Run_PrintsRuntimeTypesOfDynamicVariable()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(VarDynamicDemo.Run));

            Assert.Contains("var count = 20, type = Int32", lines);
            Assert.Contains("dynamic value = Hello, type = String", lines);
            Assert.Contains("dynamic value = 42, type = Int32", lines);
            Assert.Contains("dynamic value = True, type = Boolean", lines);
            Assert.Contains("Add(5, 7)              = 12", lines);
            Assert.Contains("Add(\"Hello, \", \"World\") = Hello, World", lines);
        }
    }

    // Task 1.14 - named tuples and deconstruction
    public class TupleTests
    {
        [Fact]
        public void GetMinMax_ReturnsNamedMinAndMax()
        {
            (int Min, int Max) result = TupleDemo.GetMinMax(new[] { 72, 95, 48, 88, 63 });

            Assert.Equal(48, result.Min);
            Assert.Equal(95, result.Max);
        }

        [Fact]
        public void GetMinMax_CanBeDeconstructed()
        {
            var (min, max) = TupleDemo.GetMinMax(new[] { -5, 0, 12 });

            Assert.Equal(-5, min);
            Assert.Equal(12, max);
        }

        [Fact]
        public void GetMinMax_SingleElement_MinEqualsMax()
        {
            var (min, max) = TupleDemo.GetMinMax(new[] { 7 });

            Assert.Equal(7, min);
            Assert.Equal(7, max);
        }

        [Theory]
        [InlineData(101, "Anita", 29, "Finance")]
        [InlineData(102, "Karthik", 35, "Engineering")]
        [InlineData(999, "Unknown", 0, "N/A")]
        public void GetEmployee_DeconstructsIntoThreeValues(int id, string expectedName, int expectedAge, string expectedDept)
        {
            var (name, age, department) = TupleDemo.GetEmployee(id);

            Assert.Equal(expectedName, name);
            Assert.Equal(expectedAge, age);
            Assert.Equal(expectedDept, department);
        }

        [Fact]
        public void Run_PrintsTupleResults()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(TupleDemo.Run));

            Assert.Contains("Using tuple names: Min = 48, Max = 95", lines);
            Assert.Contains("Deconstructed:     min = 48, max = 95", lines);
            Assert.Contains("Employee 102: Karthik, 35, Engineering", lines);
            Assert.Contains("Employee 101: Anita works in Finance", lines);
        }
    }
}
