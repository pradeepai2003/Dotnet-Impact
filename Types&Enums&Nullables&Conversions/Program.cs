using TypesEnumsNullablesConversions.ValueVsReference;
using TypesEnumsNullablesConversions.Enums;
using TypesEnumsNullablesConversions.Nullables;
using TypesEnumsNullablesConversions.Conversions;

namespace TypesEnumsNullablesConversions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 1.9: Value vs Reference types ===");
            CopyTest.Run();

            Console.WriteLine("\n=== Task 1.10: Enums & [Flags] ===");
            EnumDemo.Run();

            Console.WriteLine("\n=== Task 1.11: Nullables & ?? ===");
            NullableDemo.Run();

            Console.WriteLine("\n=== Task 1.12: Conversions ===");
            ConversionDemo.Run();
        }
    }
}
