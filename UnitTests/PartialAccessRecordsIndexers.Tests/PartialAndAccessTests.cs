using System.Reflection;
using PartialAccessRecordsIndexers.AccessModifiers;
using PartialAccessRecordsIndexers.Partial;

namespace PartialAccessRecordsIndexers.Tests
{
    // Task 1.17a - partial class + partial method
    public class EmployeeTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var emp = new Employee(7, "Deepa", 50000m);

            Assert.Equal(7, emp.Id);
            Assert.Equal("Deepa", emp.Name);
            Assert.Equal(50000m, emp.Salary);
        }

        [Fact]
        public void GetPayslip_UsesMembersFromTheOtherPartialFile()
        {
            var emp = new Employee(7, "Deepa", 50000m);

            string payslip = ConsoleCapture.WithCulture(emp.GetPayslip);

            Assert.Equal("Payslip: #7 Deepa - 50,000.00 per month", payslip);
        }

        [Fact]
        public void GiveRaise_IncreasesSalaryByPercent()
        {
            var emp = new Employee(7, "Deepa", 50000m);

            ConsoleCapture.Run(() => emp.GiveRaise(10));

            Assert.Equal(55000m, emp.Salary);
        }

        [Fact]
        public void GiveRaise_CallsPartialMethodImplementation()
        {
            var emp = new Employee(7, "Deepa", 50000m);

            string output = ConsoleCapture.Run(() => emp.GiveRaise(10));

            Assert.Contains("[audit] Deepa's salary changed 50,000.00 -> 55,000.00", output);
        }

        [Fact]
        public void PartialMethod_IsPrivate()
        {
            MethodInfo? method = typeof(Employee).GetMethod("OnSalaryChanged", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(method);
            Assert.True(method!.IsPrivate);
        }

        [Fact]
        public void PartialDemo_Run_PrintsPayslipsAndAudit()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(PartialDemo.Run));

            Assert.Contains("Payslip: #7 Deepa - 50,000.00 per month", lines);
            Assert.Contains("  [audit] Deepa's salary changed 50,000.00 -> 55,000.00", lines);
            Assert.Contains("Payslip: #7 Deepa - 55,000.00 per month", lines);
        }
    }

    // Task 1.17b - access-modifier matrix
    public class AccessModifierTests
    {
        // A derived class in ANOTHER assembly (this test project).
        private sealed class DerivedInTestAssembly : AccessMatrix
        {
            // Visible here: public, protected internal, protected.
            // NOT visible here: internal, private protected, private (different assembly).
            public string ReadVisibleMembers() => $"{PublicField}|{ProtectedInternalField}|{ProtectedField}";
        }

        [Theory]
        [InlineData("PublicField", "public")]
        [InlineData("ProtectedInternalField", "protected internal")]
        [InlineData("InternalField", "internal")]
        [InlineData("ProtectedField", "protected")]
        [InlineData("PrivateProtectedField", "private protected")]
        [InlineData("_privateField", "private")]
        public void Field_HasExpectedAccessModifier(string fieldName, string expected)
        {
            FieldInfo field = typeof(AccessMatrix).GetField(
                fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

            string actual = field switch
            {
                { IsPublic: true } => "public",
                { IsFamilyOrAssembly: true } => "protected internal",
                { IsAssembly: true } => "internal",
                { IsFamily: true } => "protected",
                { IsFamilyAndAssembly: true } => "private protected",
                { IsPrivate: true } => "private",
                _ => "unknown"
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void DerivedClassInOtherAssembly_SeesPublicAndProtectedMembers()
        {
            Assert.Equal("public|protected internal|protected", new DerivedInTestAssembly().ReadVisibleMembers());
        }

        [Fact]
        public void SameClass_SeesAllSixMembers()
        {
            string output = ConsoleCapture.Run(() => new AccessMatrix().ShowFromSameClass());

            Assert.Equal("Same class    : public, protected internal, internal, protected, private protected, private", output.Trim());
        }

        [Fact]
        public void DerivedClass_SeesFiveMembers()
        {
            string output = ConsoleCapture.Run(() => new DerivedAccess().ShowFromDerived());

            Assert.Equal("Derived class : public, protected internal, internal, protected, private protected", output.Trim());
        }

        [Fact]
        public void OtherClass_SeesThreeMembers()
        {
            string output = ConsoleCapture.Run(() => new OtherClassAccess().ShowFromOtherClass());

            Assert.Equal("Other class   : public, protected internal, internal", output.Trim());
        }

        [Fact]
        public void AccessDemo_Run_PrintsAllThreeViews()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(AccessDemo.Run));

            Assert.Equal(3, lines.Count(l => l.Length > 0));
        }
    }
}
