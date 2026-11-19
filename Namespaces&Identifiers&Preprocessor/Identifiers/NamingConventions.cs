// Task 1.7 - Naming conventions and verbatim identifiers.
namespace NamespacesIdentifiersPreprocessor.Identifiers
{
    // PascalCase for types
    public class NamingConventions
    {
        // PascalCase for methods
        public static void ShowVariables()
        {
            // camelCase for local variables
            string studentName = "Asha";
            int rollNumber = 42;
            double averageScore = 87.5;
            bool isEnrolled = true;
            DateTime admissionDate = new DateTime(2024, 6, 1);

            Console.WriteLine($"{studentName}, Roll {rollNumber}, Avg {averageScore}, " +
                              $"Enrolled {isEnrolled}, Admitted {admissionDate:yyyy-MM-dd}");

            // ---- Using a keyword as an identifier ----
            // string class = "10-A";
            //   ERROR CS1001: Identifier expected
            //   ERROR CS1002: ; expected
            //   ERROR CS1513: } expected
            //   Reason: 'class' is a reserved C# keyword and cannot be used as a name.
            //
            // FIX: prefix with '@' (verbatim identifier). The '@' is not part of the name;
            //      it just tells the compiler "treat this keyword as an identifier".
            string @class = "10-A";
            Console.WriteLine($"Class (via @class): {@class}");
        }
    }
}
