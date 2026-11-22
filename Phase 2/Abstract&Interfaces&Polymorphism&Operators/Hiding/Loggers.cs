// Task 2.6 (part 2) - Method hiding with 'new' vs overriding with 'override'.
namespace AbstractInterfacesPolymorphism.Hiding
{
    public class Logger
    {
        // NOT virtual - a derived class can only HIDE it.
        public string Log(string message) => $"[Logger] {message}";

        // virtual - a derived class can OVERRIDE it.
        public virtual string Describe() => "Base logger";
    }

    public class FileLogger : Logger
    {
        public string FilePath { get; } = "app.log";

        // 'new' hides Logger.Log. Without 'new' the code still compiles, but with
        // warning CS0108: 'FileLogger.Log(string)' hides inherited member 'Logger.Log(string)'.
        public new string Log(string message) => $"[FileLogger -> {FilePath}] {message}";

        public override string Describe() => $"File logger writing to {FilePath}";
    }

    /*
     * new vs override
     * ------------------------------------------------------------------
     *  override : the OBJECT's runtime type decides -> always FileLogger's version,
     *             even through a Logger reference. (runtime polymorphism)
     *  new      : the REFERENCE's compile-time type decides -> a Logger reference
     *             calls Logger.Log, a FileLogger reference calls FileLogger.Log.
     *             (no polymorphism - usually a sign the base method should be virtual)
     */
    public static class HidingDemo
    {
        public static void Run()
        {
            FileLogger fileLogger = new FileLogger();
            Logger asBase = fileLogger;   // same object, base-type reference

            Console.WriteLine($"FileLogger ref -> Log:      {fileLogger.Log("saved")}");
            Console.WriteLine($"Logger ref     -> Log:      {asBase.Log("saved")}        <- 'new' hides: base version runs");
            Console.WriteLine($"FileLogger ref -> Describe: {fileLogger.Describe()}");
            Console.WriteLine($"Logger ref     -> Describe: {asBase.Describe()}   <- 'override': derived version runs");
        }
    }
}
