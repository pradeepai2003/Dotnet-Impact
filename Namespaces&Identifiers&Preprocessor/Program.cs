using SchoolManagement;                               // Task 1.5: using directive
using NamespacesIdentifiersPreprocessor.Identifiers;
using NamespacesIdentifiersPreprocessor.Preprocessor;

// Task 1.6 NOTE: if we added BOTH of these:
//     using ModuleA;
//     using ModuleB;
// then calling Helper.Greet() gives:
//     ERROR CS0104: 'Helper' is an ambiguous reference between 'ModuleA.Helper' and 'ModuleB.Helper'
// So we don't import them and use fully-qualified names instead.

namespace NamespacesIdentifiersPreprocessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ---------- Task 1.5: Namespaces ----------
            Console.WriteLine("=== Task 1.5: Namespaces ===");

            // WITH the using directive: short type name is enough.
            Student s1 = new Student("Ravi", 8);
            s1.PrintDetails();

            // WITHOUT relying on the using directive: fully-qualified name.
            SchoolManagement.Student s2 = new SchoolManagement.Student("Meena", 9);
            s2.PrintDetails();

            // DIFFERENCE: Both compile to the exact same type. 'using' is only a compile-time
            // shortcut that saves typing; the fully-qualified form works even without 'using'
            // and is needed when two namespaces contain types with the same name.

            // ---------- Task 1.6: Name clash ----------
            Console.WriteLine("\n=== Task 1.6: Resolving name clash ===");
            ModuleA.Helper.Greet();
            ModuleB.Helper.Greet();

            // ---------- Task 1.7: Identifiers ----------
            Console.WriteLine("\n=== Task 1.7: Naming conventions & @class ===");
            NamingConventions.ShowVariables();

            // ---------- Task 1.8: Preprocessor ----------
            Console.WriteLine("\n=== Task 1.8: Preprocessor directives ===");
            Product product = new Product("SchoolERP");
            product.PrintLicenseInfo();
        }
    }
}
