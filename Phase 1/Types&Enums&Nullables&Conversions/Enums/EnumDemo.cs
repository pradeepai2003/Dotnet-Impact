// Task 1.10 - Enums and [Flags] enums.
namespace TypesEnumsNullablesConversions.Enums
{
    // Each name maps to an int, starting at 0 by default.
    public enum DaysOfWeek
    {
        Sunday,     // 0
        Monday,     // 1
        Tuesday,    // 2
        Wednesday,  // 3
        Thursday,   // 4
        Friday,     // 5
        Saturday    // 6
    }

    // [Flags]: values are powers of 2 (one bit each) so they can be combined.
    // Read = 001, Write = 010, Execute = 100 (binary)
    [Flags]
    public enum FilePermission
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }

    public static class EnumDemo
    {
        public static void Run()
        {
            // ---- DaysOfWeek: number 0-6 -> day name ----
            Console.Write("Enter a day number (0-6): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int dayNumber) && Enum.IsDefined(typeof(DaysOfWeek), dayNumber))
            {
                DaysOfWeek day = (DaysOfWeek)dayNumber;   // cast int -> enum
                Console.WriteLine($"Day {dayNumber} is {day}");
            }
            else
            {
                Console.WriteLine($"'{input}' is not a valid day number (0-6).");
            }

            // ---- [Flags] FilePermission ----
            // Combine with | (bitwise OR): 001 | 010 = 011
            FilePermission permissions = FilePermission.Read | FilePermission.Write;
            Console.WriteLine($"Combined permissions: {permissions}");          // Read, Write
            Console.WriteLine($"Numeric value: {(int)permissions}");            // 3

            // Test a single permission with & (bitwise AND): keeps only the bits both share.
            bool canWrite = (permissions & FilePermission.Write) == FilePermission.Write;
            bool canExecute = (permissions & FilePermission.Execute) == FilePermission.Execute;
            Console.WriteLine($"Can write?   {canWrite}");     // True
            Console.WriteLine($"Can execute? {canExecute}");   // False

            // Add a permission later with |=
            permissions |= FilePermission.Execute;
            Console.WriteLine($"After adding Execute: {permissions}");          // Read, Write, Execute
        }
    }
}
