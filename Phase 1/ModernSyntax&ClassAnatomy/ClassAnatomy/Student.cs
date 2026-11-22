// Task 1.16 - Class anatomy: validated properties, chained constructors,
// overloaded methods, const vs readonly.
namespace ModernSyntaxClassAnatomy.ClassAnatomy
{
    public class Student
    {
        #region Fields
        // const: value fixed at COMPILE time, implicitly static, same for every Student.
        //        Can only be assigned here, at declaration.
        public const string SchoolName = "Greenwood High";

        // readonly: value fixed at RUN time, per object. Can be assigned at declaration
        //           OR inside a constructor - and never again after that.
        public readonly int StudentId;
        public readonly DateTime EnrolledOn = DateTime.Today;

        private static int _nextId = 1;
        private int _age;
        #endregion

        #region Properties
        public string Name { get; }

        // Validated property: rejects ages outside 5-100.
        public int Age
        {
            get => _age;
            set
            {
                if (value < 5 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(Age), value, "Age must be between 5 and 100.");
                _age = value;
            }
        }
        #endregion

        #region Constructors
        // Main (parameterized) constructor - all real initialization happens here.
        public Student(string name, int age)
        {
            StudentId = _nextId++;   // OK: readonly can be assigned in a constructor
            Name = name;
            Age = age;               // goes through the validation in the setter
            // SchoolName = "Other"; // ERROR CS0131: const can't be assigned anywhere but its declaration
        }

        // Chained constructor: ': this(...)' calls the constructor above first,
        // so the ID/validation logic is written once.
        public Student(string name) : this(name, 18)
        {
            Console.WriteLine($"  (chained ctor used: default age 18 applied for {name})");
        }
        #endregion

        #region Methods
        // Overload 1: a single score out of 100
        public string CalculateGrade(int marks) => ToGrade(marks);

        // Overload 2: obtained out of a custom total
        public string CalculateGrade(int obtained, int total) => ToGrade(obtained * 100.0 / total);

        // Overload 3: average of several subjects
        public string CalculateGrade(int[] subjectMarks) => ToGrade(subjectMarks.Average());

        private static string ToGrade(double percent) => percent switch
        {
            >= 90 => "A",
            >= 75 => "B",
            >= 60 => "C",
            >= 40 => "D",
            _     => "F"
        };

        public void TryChangeId()
        {
            // StudentId = 999;
            //   ERROR CS0191: A readonly field cannot be assigned to
            //   (except in a constructor or init-only setter of the type in which the field is defined)
        }

        public override string ToString() =>
            $"#{StudentId} {Name}, Age {Age}, {SchoolName}, enrolled {EnrolledOn:yyyy-MM-dd}";
        #endregion
    }

    public static class StudentDemo
    {
        public static void Run()
        {
            // Parameterized constructor
            Student s1 = new Student("Priya", 15);
            Console.WriteLine(s1);

            // Chained constructor
            Student s2 = new Student("Arjun");
            Console.WriteLine(s2);

            // Validation: out-of-range age is rejected
            try
            {
                Student bad = new Student("Tiny", 3);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Rejected: {ex.Message}");
            }
            try
            {
                s1.Age = 150;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Rejected: {ex.Message}");
            }
            Console.WriteLine($"s1.Age is still {s1.Age}");

            // Overloaded CalculateGrade
            Console.WriteLine($"CalculateGrade(92)           = {s1.CalculateGrade(92)}");
            Console.WriteLine($"CalculateGrade(45, 60)       = {s1.CalculateGrade(45, 60)}");
            Console.WriteLine($"CalculateGrade([70, 80, 65]) = {s1.CalculateGrade(new[] { 70, 80, 65 })}");

            // const vs readonly
            Console.WriteLine($"const    SchoolName = {Student.SchoolName} (accessed via the class - shared by all)");
            Console.WriteLine($"readonly StudentId  = {s1.StudentId} vs {s2.StudentId} (different per object)");
        }
    }
}
