// Task 1.17 - Access-modifier matrix.
//
//  Modifier             | Same class | Derived (same project) | Other class (same project) | Derived (other project) | Other (other project)
//  ---------------------|------------|------------------------|----------------------------|-------------------------|----------------------
//  public               |    Yes     |          Yes           |            Yes             |           Yes           |          Yes
//  protected internal   |    Yes     |          Yes           |            Yes             |           Yes           |          No
//  internal             |    Yes     |          Yes           |            Yes             |           No            |          No
//  protected            |    Yes     |          Yes           |            No              |           Yes           |          No
//  private protected    |    Yes     |          Yes           |            No              |           No            |          No
//  private              |    Yes     |          No            |            No              |           No            |          No
//
// Defaults: class members are 'private'; top-level classes are 'internal'.
namespace PartialAccessRecordsIndexers.AccessModifiers
{
    public class AccessMatrix
    {
        public string PublicField = "public";
        protected internal string ProtectedInternalField = "protected internal";
        internal string InternalField = "internal";
        protected string ProtectedField = "protected";
        private protected string PrivateProtectedField = "private protected";
        private string _privateField = "private";

        // Same class: everything is accessible.
        public void ShowFromSameClass()
        {
            Console.WriteLine($"Same class    : {PublicField}, {ProtectedInternalField}, {InternalField}, " +
                              $"{ProtectedField}, {PrivateProtectedField}, {_privateField}");
        }
    }

    // Derived class in the same project.
    public class DerivedAccess : AccessMatrix
    {
        public void ShowFromDerived()
        {
            Console.WriteLine($"Derived class : {PublicField}, {ProtectedInternalField}, {InternalField}, " +
                              $"{ProtectedField}, {PrivateProtectedField}");
            // Console.WriteLine(_privateField);
            //   ERROR CS0122: 'AccessMatrix._privateField' is inaccessible due to its protection level
        }
    }

    // Unrelated class in the same project.
    public class OtherClassAccess
    {
        public void ShowFromOtherClass()
        {
            var m = new AccessMatrix();
            Console.WriteLine($"Other class   : {m.PublicField}, {m.ProtectedInternalField}, {m.InternalField}");
            // m.ProtectedField / m.PrivateProtectedField / m._privateField
            //   ERROR CS0122: inaccessible due to its protection level (not derived)
        }
    }

    public static class AccessDemo
    {
        public static void Run()
        {
            new AccessMatrix().ShowFromSameClass();
            new DerivedAccess().ShowFromDerived();
            new OtherClassAccess().ShowFromOtherClass();
        }
    }
}
