using TypesEnumsNullablesConversions.ValueVsReference;

namespace TypesEnumsNullablesConversions.Tests
{
    // Task 1.9 - value vs reference semantics
    public class ValueVsReferenceTests
    {
        [Fact]
        public void Struct_Copy_IsIndependent()
        {
            var s1 = new CoordinateStruct(1, 2);
            CoordinateStruct s2 = s1;

            s2.X = 99;

            Assert.Equal(1, s1.X);
            Assert.Equal(99, s2.X);
            Assert.Equal(2, s1.Y);
        }

        [Fact]
        public void Class_Copy_SharesTheSameObject()
        {
            var c1 = new CoordinateClass(1, 2);
            CoordinateClass c2 = c1;

            c2.X = 99;

            Assert.Equal(99, c1.X);
            Assert.Same(c1, c2);
            Assert.Equal(2, c1.Y);
        }

        [Fact]
        public void CoordinateStruct_IsValueType_CoordinateClass_IsNot()
        {
            Assert.True(typeof(CoordinateStruct).IsValueType);
            Assert.False(typeof(CoordinateClass).IsValueType);
        }

        [Fact]
        public void Run_PrintsExpectedResultForEachType()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(CopyTest.Run));

            Assert.Contains("int:    a = 10, b = 99", lines);
            Assert.Contains("int[]:  arr1[0] = 99, arr2[0] = 99", lines);
            Assert.Contains("struct: s1.X = 1, s2.X = 99", lines);
            Assert.Contains("class:  c1.X = 99, c2.X = 99", lines);
        }
    }
}
