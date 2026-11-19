using ModernSyntaxClassAnatomy.VarAndDynamic;
using ModernSyntaxClassAnatomy.Tuples;
using ModernSyntaxClassAnatomy.PatternMatching;
using ModernSyntaxClassAnatomy.ClassAnatomy;

namespace ModernSyntaxClassAnatomy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 1.13: var vs dynamic ===");
            VarDynamicDemo.Run();

            Console.WriteLine("\n=== Task 1.14: Tuples & deconstruction ===");
            TupleDemo.Run();

            Console.WriteLine("\n=== Task 1.15: Pattern matching ===");
            PatternDemo.Run();

            Console.WriteLine("\n=== Task 1.16: Class anatomy ===");
            StudentDemo.Run();
        }
    }
}
