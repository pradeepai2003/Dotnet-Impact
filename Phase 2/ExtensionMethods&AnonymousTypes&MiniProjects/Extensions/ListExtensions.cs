// Task 2.14 - Generic extension method on List<T>.
namespace ExtensionsMiniProjects.Extensions
{
    public static class ListExtensions
    {
        // Works even when the list itself is null: extension methods are static calls,
        // so list.IsNullOrEmpty() on a null list does NOT throw NullReferenceException.
        public static bool IsNullOrEmpty<T>(this List<T>? list) => list is null || list.Count == 0;
    }
}
