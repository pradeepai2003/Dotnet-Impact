// Task 2.14 - Extension method on string.
// An extension method is a STATIC method in a STATIC class whose first parameter uses 'this'.
// It can then be called as if it were an instance method: "hello world".ToTitleCase().
using System.Text;

namespace ExtensionsMiniProjects.Extensions
{
    public static class StringExtensions
    {
        // "hELLO   wORLD" -> "Hello   World". Null or empty -> "". Whitespace is kept as-is.
        public static string ToTitleCase(this string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var result = new StringBuilder(text.Length);
            bool startOfWord = true;

            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                {
                    result.Append(c);
                    startOfWord = true;
                }
                else if (startOfWord)
                {
                    result.Append(char.ToUpperInvariant(c));
                    startOfWord = false;
                }
                else
                {
                    result.Append(char.ToLowerInvariant(c));
                }
            }
            return result.ToString();
        }
    }
}
