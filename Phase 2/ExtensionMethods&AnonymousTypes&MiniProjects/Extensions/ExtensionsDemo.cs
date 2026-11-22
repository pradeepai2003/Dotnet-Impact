// Task 2.14 - Using the three extension methods.
namespace ExtensionsMiniProjects.Extensions
{
    public static class ExtensionsDemo
    {
        public static void Run()
        {
            foreach (string? text in new[] { "hello world", "hELLO   wORLD", "c# is FUN", "", null })
            {
                Console.WriteLine($"ToTitleCase({Show(text)}) = \"{text.ToTitleCase()}\"");
            }

            List<int>? nullList = null;
            Console.WriteLine($"null list IsNullOrEmpty   = {nullList.IsNullOrEmpty()}");
            Console.WriteLine($"empty list IsNullOrEmpty  = {new List<string>().IsNullOrEmpty()}");
            Console.WriteLine($"[1, 2] IsNullOrEmpty      = {new List<int> { 1, 2 }.IsNullOrEmpty()}");

            foreach (int n in new[] { 0, 7, 13, 20, 45, 100, 105, 999 })
            {
                Console.WriteLine($"{n,3}.ToWords() = {n.ToWords()}");
            }
        }

        private static string Show(string? text) => text is null ? "null" : $"\"{text}\"";
    }
}
