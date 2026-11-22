using ExtensionsMiniProjects.AnonymousTypes;
using ExtensionsMiniProjects.Extensions;
using ExtensionsMiniProjects.MiniProjects.Library;
using ExtensionsMiniProjects.MiniProjects.Notifications;
using ExtensionsMiniProjects.MiniProjects.Payroll;

namespace ExtensionsMiniProjects
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Task 2.14: Extension methods ===");
            ExtensionsDemo.Run();

            Console.WriteLine("\n=== Task 2.14: Anonymous types ===");
            AnonymousTypeDemo.Run();

            Console.WriteLine("\n=== Mini Q4: Employee Payroll ===");
            PayrollDemo.Run();

            Console.WriteLine("\n=== Mini Q5: Notification Engine ===");
            NotificationEngineDemo.Run();

            Console.WriteLine("\n=== Mini Q6: Library Management (LINQ) ===");
            LibraryDemo.Run();
        }
    }
}
