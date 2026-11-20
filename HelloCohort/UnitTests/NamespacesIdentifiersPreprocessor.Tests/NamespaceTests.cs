using SchoolManagement;

namespace NamespacesIdentifiersPreprocessor.Tests
{
    // Task 1.5 - SchoolManagement.Student
    public class StudentTests
    {
        [Fact]
        public void Constructor_SetsNameAndGrade()
        {
            var student = new Student("Ravi", 8);

            Assert.Equal("Ravi", student.Name);
            Assert.Equal(8, student.Grade);
        }

        [Fact]
        public void PrintDetails_WritesNameAndGrade()
        {
            string output = ConsoleCapture.Run(() => new Student("Meena", 9).PrintDetails());

            Assert.Equal("Student: Meena, Grade: 9", output.Trim());
        }

        [Fact]
        public void ShortNameAndFullyQualifiedName_AreTheSameType()
        {
            // 'Student' (via using) and 'SchoolManagement.Student' compile to one type.
            Assert.Equal(typeof(SchoolManagement.Student), typeof(Student));
            Assert.Equal("SchoolManagement", typeof(Student).Namespace);
        }
    }

    // Task 1.6 - ModuleA.Helper vs ModuleB.Helper
    public class HelperTests
    {
        [Fact]
        public void ModuleA_Greet_WritesModuleAMessage()
        {
            string output = ConsoleCapture.Run(ModuleA.Helper.Greet);

            Assert.Equal("Hello from ModuleA.Helper.Greet()", output.Trim());
        }

        [Fact]
        public void ModuleB_Greet_WritesModuleBMessage()
        {
            string output = ConsoleCapture.Run(ModuleB.Helper.Greet);

            Assert.Equal("Hello from ModuleB.Helper.Greet()", output.Trim());
        }

        [Fact]
        public void TwoHelpers_SameName_AreDifferentTypes()
        {
            Assert.Equal("Helper", typeof(ModuleA.Helper).Name);
            Assert.Equal("Helper", typeof(ModuleB.Helper).Name);
            Assert.NotEqual(typeof(ModuleA.Helper), typeof(ModuleB.Helper));
        }
    }
}
