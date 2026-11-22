// Task 2.12 - Lazy iterators with 'yield return' and a custom IEnumerable<T>.
using System.Collections;

namespace GenericsYieldLinq.Yield
{
    public static class NumberSequences
    {
        // NOT an iterator itself: it validates IMMEDIATELY, then hands off to the iterator.
        // (Inside an iterator, even the argument check would be deferred until the first MoveNext.)
        public static IEnumerable<int> GetEvenNumbers(int max, Action<int>? onYield = null)
        {
            if (max < 0)
                throw new ArgumentOutOfRangeException(nameof(max), max, "max cannot be negative.");
            return Iterate(max, onYield);

            static IEnumerable<int> Iterate(int max, Action<int>? onYield)
            {
                for (int n = 2; n <= max; n += 2)
                {
                    onYield?.Invoke(n);   // lets callers SEE when each value is produced
                    yield return n;       // pause here until the caller asks for the next value
                }
            }
        }
    }

    public record Book(string Title, string Author);

    // Implementing IEnumerable<Book> makes the collection usable in foreach and LINQ.
    public class BookCollection : IEnumerable<Book>
    {
        private readonly List<Book> _books = new();

        public int Count => _books.Count;

        public void Add(Book book)
        {
            ArgumentNullException.ThrowIfNull(book);
            _books.Add(book);
        }

        // Yields books alphabetically by title, one at a time.
        public IEnumerator<Book> GetEnumerator()
        {
            foreach (Book book in _books.OrderBy(b => b.Title, StringComparer.OrdinalIgnoreCase))
            {
                yield return book;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class IteratorDemo
    {
        public static void Run()
        {
            Console.WriteLine("Lazy consumption of GetEvenNumbers(20), stopping after 6:");
            foreach (int n in NumberSequences.GetEvenNumbers(20, onYield: v => Console.WriteLine($"  produced {v}")))
            {
                Console.WriteLine($"  consumed {n}");
                if (n == 6) break;   // 8..20 are never produced
            }

            var books = new BookCollection
            {
                new Book("The Pragmatic Programmer", "Andrew Hunt"),
                new Book("Clean Code", "Robert C. Martin"),
                new Book("refactoring", "Martin Fowler"),
                new Book("Design Patterns", "Erich Gamma"),
            };
            Console.WriteLine("Books (alphabetical):");
            foreach (Book book in books)
                Console.WriteLine($"  {book.Title} - {book.Author}");
        }
    }
}
