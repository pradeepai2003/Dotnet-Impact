using DelegatesEventsLambdas.BuiltInDelegates;
using DelegatesEventsLambdas.Delegates;

namespace DelegatesEventsLambdas.Tests
{
    // Task 2.8 - custom delegate, multicast, Func
    public class MathOperationTests
    {
        [Theory]
        [InlineData(10, 5, 15, 5, 50, 2)]
        [InlineData(-3, 2, -1, -5, -6, -1.5)]
        public void EachOperation_ThroughTheDelegate(double a, double b, double sum, double diff, double product, double quotient)
        {
            MathOperation add = MathOperations.Add, sub = MathOperations.Subtract, mul = MathOperations.Multiply, div = MathOperations.Divide;

            Assert.Equal(sum, add(a, b));
            Assert.Equal(diff, sub(a, b));
            Assert.Equal(product, mul(a, b));
            Assert.Equal(quotient, div(a, b));
        }

        [Fact]
        public void Divide_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => MathOperations.Divide(1, 0));
        }

        [Fact]
        public void Multicast_RunsBothMethods()
        {
            MathOperation multicast = MathOperations.Add;
            multicast += MathOperations.Multiply;

            Assert.Equal(2, multicast.GetInvocationList().Length);
            Assert.Equal(new[] { 15.0, 50.0 }, MathOperations.InvokeAll(multicast, 10, 5));
        }

        [Fact]
        public void Multicast_DirectInvoke_ReturnsLastResult()
        {
            MathOperation multicast = MathOperations.Add;
            multicast += MathOperations.Multiply;

            Assert.Equal(50, multicast(10, 5));
        }

        [Fact]
        public void Multicast_RemovingAMethod()
        {
            MathOperation? multicast = MathOperations.Add;
            multicast += MathOperations.Multiply;
            multicast -= MathOperations.Add;   // -= can leave null when the last method is removed

            Assert.NotNull(multicast);
            Assert.Equal(new[] { 50.0 }, MathOperations.InvokeAll(multicast!, 10, 5));
        }

        [Fact]
        public void InvokeAll_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => MathOperations.InvokeAll(null!, 1, 2));
        }

        [Theory]
        [InlineData(7, 3)]
        [InlineData(-4.5, 2)]
        [InlineData(0, 9)]
        public void FuncVersion_MatchesDelegateVersion_ForAllFour(double a, double b)
        {
            MathOperation add = MathOperations.Add, sub = MathOperations.Subtract, mul = MathOperations.Multiply, div = MathOperations.Divide;

            Assert.Equal(add(a, b), DelegateDemo.AddFunc(a, b));
            Assert.Equal(sub(a, b), DelegateDemo.SubtractFunc(a, b));
            Assert.Equal(mul(a, b), DelegateDemo.MultiplyFunc(a, b));
            Assert.Equal(div(a, b), DelegateDemo.DivideFunc(a, b));
        }

        [Fact]
        public void DivideFunc_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => DelegateDemo.DivideFunc(1, 0));
        }

        [Fact]
        public void DelegateDemo_Run_ShowsMulticastAndFunc()
        {
            string output = ConsoleCapture.Run(DelegateDemo.Run);

            Assert.Contains("Multicast methods: Add + Multiply", output);
            Assert.Contains("Each method's result: 15, 50", output);
            Assert.Contains("Func Add     = 15 (matches delegate: True)", output);
            Assert.Contains("Func Subtract= 5 (matches delegate: True)", output);
            Assert.Contains("Func Multiply= 50 (matches delegate: True)", output);
            Assert.Contains("Func Divide  = 2 (matches delegate: True)", output);
            Assert.Contains("Func multicast methods: Add + Multiply", output);
        }
    }

    // Task 2.10 - Action / Func / Predicate
    public class BuiltInDelegateTests
    {
        [Fact]
        public void Action_PrintsUppercase()
        {
            Assert.Equal("HELLO", ConsoleCapture.Run(() => StandardDelegates.PrintUpper("hello")).Trim());
        }

        [Fact]
        public void Func_Multiplies() => Assert.Equal(42, StandardDelegates.Multiply(6, 7));

        [Theory]
        [InlineData(4, true)]
        [InlineData(0, true)]
        [InlineData(7, false)]
        [InlineData(-3, false)]
        public void Predicate_IsEven(int n, bool expected) => Assert.Equal(expected, StandardDelegates.IsEven(n));

        [Fact]
        public void ProcessList_FilterEvens_Square_Print()
        {
            var printed = new List<int>();

            List<int> result = ListProcessor.ProcessList(
                Enumerable.Range(1, 10), StandardDelegates.IsEven, n => StandardDelegates.Multiply(n, n), printed.Add);

            Assert.Equal(new[] { 4, 16, 36, 64, 100 }, result);
            Assert.Equal(result, printed);   // output called once per item, in order
        }

        [Fact]
        public void ProcessList_NoMatches_ReturnsEmptyAndPrintsNothing()
        {
            int calls = 0;

            List<int> result = ListProcessor.ProcessList(new[] { 1, 3, 5 }, StandardDelegates.IsEven, n => n, _ => calls++);

            Assert.Empty(result);
            Assert.Equal(0, calls);
        }

        [Fact]
        public void ProcessList_EmptyInput_ReturnsEmpty()
        {
            Assert.Empty(ListProcessor.ProcessList(Array.Empty<int>(), _ => true, n => n, _ => { }));
        }

        [Fact]
        public void ProcessList_NullArguments_Throw()
        {
            int[] list = { 1 };
            Assert.Throws<ArgumentNullException>(() => ListProcessor.ProcessList(null!, _ => true, n => n, _ => { }));
            Assert.Throws<ArgumentNullException>(() => ListProcessor.ProcessList(list, null!, n => n, _ => { }));
            Assert.Throws<ArgumentNullException>(() => ListProcessor.ProcessList(list, _ => true, null!, _ => { }));
            Assert.Throws<ArgumentNullException>(() => ListProcessor.ProcessList(list, _ => true, n => n, null!));
        }

        [Fact]
        public void Demo_Run_PrintsExpectedSequence()
        {
            string output = ConsoleCapture.Run(BuiltInDelegatesDemo.Run);

            Assert.Contains("HELLO FROM ACTION<STRING>", output);
            Assert.Contains("Multiply(6, 7) = 42", output);
            Assert.Contains("IsEven(4) = True, IsEven(7) = False", output);
            Assert.Contains("Result: [4, 16, 36, 64, 100]", output);
        }
    }
}
