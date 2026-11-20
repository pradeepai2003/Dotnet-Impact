using NamespacesIdentifiersPreprocessor.Identifiers;
using NamespacesIdentifiersPreprocessor.Preprocessor;

namespace NamespacesIdentifiersPreprocessor.Tests
{
    // Task 1.7 - naming conventions and @class
    public class NamingConventionsTests
    {
        [Fact]
        public void ShowVariables_PrintsAllFiveVariables()
        {
            string output = ConsoleCapture.Run(NamingConventions.ShowVariables);

            Assert.Contains("Asha, Roll 42, Avg 87.5, Enrolled True, Admitted 2024-06-01", output);
        }

        [Fact]
        public void ShowVariables_PrintsVerbatimIdentifierValue()
        {
            string output = ConsoleCapture.Run(NamingConventions.ShowVariables);

            Assert.Contains("Class (via @class): 10-A", output);
        }

        [Fact]
        public void VerbatimIdentifier_NameDoesNotIncludeAtSign()
        {
            string @class = "10-A";

            Assert.Equal("class", nameof(@class));
            Assert.Equal("10-A", @class);
        }
    }

    // Task 1.8 - #define TRIAL_VERSION / #if / #region
    // These tests describe the TRIAL build. If '#define TRIAL_VERSION' is commented out
    // in TrialDemo.cs, they fail on purpose - proving the directive changes the build.
    public class ProductTests
    {
        [Fact]
        public void Constructor_SetsName()
        {
            var product = new Product("SchoolERP");

            Assert.Equal("SchoolERP", product.Name);
        }

        [Fact]
        public void TrialVersion_LimitsUsersToFive()
        {
            var product = new Product("SchoolERP");

            Assert.Equal(5, product.MaxUsers);
        }

        [Fact]
        public void PrintLicenseInfo_PrintsTrialMessage()
        {
            string output = ConsoleCapture.Run(() => new Product("SchoolERP").PrintLicenseInfo());

            Assert.Equal("SchoolERP: TRIAL VERSION - limited to 5 users. Please upgrade!", output.Trim());
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => NamespacesIdentifiersPreprocessor.Program.Main(Array.Empty<string>()));

            string[] expectedInOrder =
            {
                "=== Task 1.5: Namespaces ===",
                "Student: Ravi, Grade: 8",
                "Student: Meena, Grade: 9",
                "=== Task 1.6: Resolving name clash ===",
                "Hello from ModuleA.Helper.Greet()",
                "Hello from ModuleB.Helper.Greet()",
                "=== Task 1.7: Naming conventions & @class ===",
                "Class (via @class): 10-A",
                "=== Task 1.8: Preprocessor directives ===",
                "SchoolERP: TRIAL VERSION - limited to 5 users. Please upgrade!",
            };

            int position = -1;
            foreach (string expected in expectedInOrder)
            {
                int next = output.IndexOf(expected, StringComparison.Ordinal);
                Assert.True(next > position, $"Expected \"{expected}\" after position {position}.");
                position = next;
            }
        }
    }
}
