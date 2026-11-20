// Task 1.5 - A class inside its own namespace.
namespace SchoolManagement
{
    public class Student
    {
        public string Name { get; }
        public int Grade { get; }

        public Student(string name, int grade)
        {
            Name = name;
            Grade = grade;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"Student: {Name}, Grade: {Grade}");
        }
    }
}
