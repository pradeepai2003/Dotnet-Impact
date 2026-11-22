// Task 1.8 - Preprocessor directives.
// #define must appear before any other code in the file.
// Comment out the next line to switch to the FULL version output.
#define TRIAL_VERSION

namespace NamespacesIdentifiersPreprocessor.Preprocessor
{
    public class Product
    {
        #region Fields
        private readonly string _name;
        private readonly int _maxUsers;
        #endregion

        #region Properties
        public string Name => _name;
        public int MaxUsers => _maxUsers;
        #endregion

        #region Constructors
        public Product(string name)
        {
            _name = name;
#if TRIAL_VERSION
            _maxUsers = 5;
#else
            _maxUsers = 1000;
#endif
        }
        #endregion

        #region Methods
        public void PrintLicenseInfo()
        {
#if TRIAL_VERSION
            Console.WriteLine($"{Name}: TRIAL VERSION - limited to {MaxUsers} users. Please upgrade!");
#else
            Console.WriteLine($"{Name}: FULL VERSION - licensed for {MaxUsers} users. Thank you!");
#endif
        }
        #endregion
    }
}
