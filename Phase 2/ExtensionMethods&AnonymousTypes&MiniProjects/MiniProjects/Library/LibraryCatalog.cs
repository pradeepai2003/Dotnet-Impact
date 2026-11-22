// Mini Q6 - Library Management with LINQ.
namespace ExtensionsMiniProjects.MiniProjects.Library
{
    public record Book(string Title, string Author, string Genre, int Year, bool IsAvailable);

    public record AuthorBooks(string Author, IReadOnlyList<string> Titles);

    public record GenreCount(string Genre, int Count);

    public class LibraryCatalog
    {
        private readonly List<Book> _books;

        public LibraryCatalog(IEnumerable<Book> books)
        {
            ArgumentNullException.ThrowIfNull(books);
            _books = books.ToList();
        }

        public IReadOnlyList<Book> Books => _books.AsReadOnly();

        // 1) Available books grouped by author (authors A-Z, titles A-Z).
        public List<AuthorBooks> AvailableBooksByAuthor() =>
            _books.Where(b => b.IsAvailable)
                  .GroupBy(b => b.Author)
                  .OrderBy(g => g.Key, StringComparer.Ordinal)
                  .Select(g => new AuthorBooks(g.Key, g.Select(b => b.Title).OrderBy(t => t, StringComparer.Ordinal).ToList()))
                  .ToList();

        // 2) Available books by one author (case-insensitive).
        public List<Book> AvailableBooksBy(string author) =>
            _books.Where(b => b.IsAvailable && b.Author.Equals(author, StringComparison.OrdinalIgnoreCase))
                  .OrderBy(b => b.Title, StringComparer.Ordinal)
                  .ToList();

        // 3) Group by genre with count (largest first, then genre A-Z).
        public List<GenreCount> CountByGenre() =>
            (from b in _books
             group b by b.Genre into g
             orderby g.Count() descending, g.Key
             select new GenreCount(g.Key, g.Count())).ToList();

        // 4) Oldest book (null when the library is empty).
        public Book? OldestBook() =>
            _books.OrderBy(b => b.Year).ThenBy(b => b.Title, StringComparer.Ordinal).FirstOrDefault();

        // 5) Books published after a year, sorted by title.
        public List<Book> PublishedAfter(int year) =>
            _books.Where(b => b.Year > year)
                  .OrderBy(b => b.Title, StringComparer.Ordinal)
                  .ToList();

        // 6) Title search (case-insensitive). Blank keyword -> no results.
        public List<Book> SearchByTitle(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Book>();

            return _books.Where(b => b.Title.Contains(keyword.Trim(), StringComparison.OrdinalIgnoreCase))
                         .OrderBy(b => b.Title, StringComparer.Ordinal)
                         .ToList();
        }

        public static LibraryCatalog CreateSample() => new(new[]
        {
            new Book("Clean Code", "Robert C. Martin", "Programming", 2008, true),
            new Book("The Clean Coder", "Robert C. Martin", "Programming", 2011, false),
            new Book("Clean Architecture", "Robert C. Martin", "Programming", 2017, true),
            new Book("Refactoring", "Martin Fowler", "Programming", 1999, true),
            new Book("The Pragmatic Programmer", "Andrew Hunt", "Programming", 1999, false),
            new Book("Pride and Prejudice", "Jane Austen", "Fiction", 1813, true),
            new Book("Emma", "Jane Austen", "Fiction", 1815, false),
            new Book("1984", "George Orwell", "Fiction", 1949, true),
            new Book("Animal Farm", "George Orwell", "Fiction", 1945, true),
            new Book("Sapiens", "Yuval Noah Harari", "History", 2011, true),
            new Book("Homo Deus", "Yuval Noah Harari", "History", 2015, false),
            new Book("Guns, Germs, and Steel", "Jared Diamond", "History", 1997, true),
            new Book("A Brief History of Time", "Stephen Hawking", "Science", 1988, true),
            new Book("The Gene", "Siddhartha Mukherjee", "Science", 2016, true),
            new Book("Cosmos", "Carl Sagan", "Science", 1980, false),
            new Book("Atomic Habits", "James Clear", "Self-Help", 2018, true),
        });
    }

    public static class LibraryDemo
    {
        public static void Run()
        {
            LibraryCatalog library = LibraryCatalog.CreateSample();
            Console.WriteLine($"Library has {library.Books.Count} books.");

            Console.WriteLine("1) Available books by author:");
            foreach (AuthorBooks a in library.AvailableBooksByAuthor())
                Console.WriteLine($"   {a.Author}: {string.Join(", ", a.Titles)}");

            Console.WriteLine($"2) Available by 'robert c. martin': {string.Join(", ", library.AvailableBooksBy("robert c. martin").Select(b => b.Title))}");

            Console.WriteLine("3) Books per genre:");
            foreach (GenreCount g in library.CountByGenre())
                Console.WriteLine($"   {g.Genre,-12} {g.Count}");

            Book? oldest = library.OldestBook();
            Console.WriteLine($"4) Oldest book: {oldest?.Title} ({oldest?.Year})");

            Console.WriteLine($"5) After 2010 (by title): {string.Join(", ", library.PublishedAfter(2010).Select(b => $"{b.Title} ({b.Year})"))}");

            Console.WriteLine($"6) Title contains 'clean': {string.Join(", ", library.SearchByTitle("clean").Select(b => b.Title))}");
        }
    }
}
