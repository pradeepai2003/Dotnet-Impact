// Task 1.9 - Copy-and-modify test: value types vs reference types.
namespace TypesEnumsNullablesConversions.ValueVsReference
{
    // struct = VALUE type: the variable holds the data itself.
    public struct CoordinateStruct
    {
        public int X;
        public int Y;
        public CoordinateStruct(int x, int y) { X = x; Y = y; }
    }

    // class = REFERENCE type: the variable holds a reference (address) to an object on the heap.
    public class CoordinateClass
    {
        public int X;
        public int Y;
        public CoordinateClass(int x, int y) { X = x; Y = y; }
    }

    public static class CopyTest
    {
        public static void Run()
        {
            // ---- 1. Two ints ----
            int a = 10;
            int b = a;      // copies the VALUE 10 into b
            b = 99;
            Console.WriteLine($"int:    a = {a}, b = {b}");
            // OUTPUT: a = 10, b = 99
            // WHY: int is a value type. 'b = a' made an independent copy of the number,
            //      so changing b has no effect on a.

            // ---- 2. int[] ----
            int[] arr1 = { 1, 2, 3 };
            int[] arr2 = arr1;  // copies the REFERENCE, not the array
            arr2[0] = 99;
            Console.WriteLine($"int[]:  arr1[0] = {arr1[0]}, arr2[0] = {arr2[0]}");
            // OUTPUT: arr1[0] = 99, arr2[0] = 99
            // WHY: arrays are reference types (even an array of ints). 'arr2 = arr1' copied
            //      only the reference, so both variables point to the SAME array in memory.
            //      Changing it through arr2 is visible through arr1 -> SHARED state.

            // ---- 3. struct ----
            CoordinateStruct s1 = new CoordinateStruct(1, 2);
            CoordinateStruct s2 = s1;   // copies all fields (X and Y) into a new struct
            s2.X = 99;
            Console.WriteLine($"struct: s1.X = {s1.X}, s2.X = {s2.X}");
            // OUTPUT: s1.X = 1, s2.X = 99
            // WHY: struct is a value type, like int. Assignment copies the whole value,
            //      so s1 and s2 are two separate coordinates -> NOT shared.

            // ---- 4. class ----
            CoordinateClass c1 = new CoordinateClass(1, 2);
            CoordinateClass c2 = c1;    // copies the reference only
            c2.X = 99;
            Console.WriteLine($"class:  c1.X = {c1.X}, c2.X = {c2.X}");
            // OUTPUT: c1.X = 99, c2.X = 99
            // WHY: class is a reference type, like the array. c1 and c2 point to the SAME
            //      object, so a change through c2 is seen through c1 -> SHARED state.

            // SUMMARY: value types (int, struct) copy the DATA -> independent copies.
            //          reference types (array, class) copy the REFERENCE -> shared object.
        }
    }
}
