using ExtensionsMiniProjects.MiniProjects.Library;

namespace ExtensionsMiniProjects.Tests
{
    // Mini Q6 - Library Management (six LINQ queries)
    public class LibraryTests
    {
        private readonly LibraryCatalog _library = LibraryCatalog.CreateSample();

        [Fact]
        public void Sample_HasAtLeastFifteenBooks()
        {
            Assert.True(_library.Books.Count >= 15);
        }

        // ---- 1. available books grouped by author ----
        [Fact]
        public void AvailableBooksByAuthor_GroupsOnlyAvailableBooks()
        {
            List<AuthorBooks> groups = _library.AvailableBooksByAuthor();

            Assert.Equal(9, groups.Count);
            Assert.Equal("George Orwell", groups[0].Author);                         // authors sorted A-Z
            Assert.Equal(new[] { "1984", "Animal Farm" }, groups[0].Titles);
            Assert.Equal(new[] { "Clean Architecture", "Clean Code" },
                         groups.Single(g => g.Author == "Robert C. Martin").Titles);  // "The Clean Coder" is out
            Assert.DoesNotContain(groups, g => g.Author == "Andrew Hunt");             // only book checked out
            Assert.DoesNotContain(groups, g => g.Author == "Carl Sagan");
            Assert.Equal(11, groups.Sum(g => g.Titles.Count));
        }

        // ---- 2. available books by one author ----
        [Theory]
        [InlineData("Robert C. Martin", 2)]
        [InlineData("robert c. martin", 2)]   // case-insensitive
        [InlineData("Jane Austen", 1)]
        [InlineData("Andrew Hunt", 0)]        // author exists, nothing available
        [InlineData("Unknown Author", 0)]
        public void AvailableBooksBy_Author(string author, int expected)
        {
            List<Book> books = _library.AvailableBooksBy(author);

            Assert.Equal(expected, books.Count);
            Assert.All(books, b => Assert.True(b.IsAvailable));
        }

        // ---- 3. group by genre with count ----
        [Fact]
        public void CountByGenre_LargestFirst_ThenAlphabetical()
        {
            Assert.Equal(new[]
            {
                new GenreCount("Programming", 5),
                new GenreCount("Fiction", 4),
                new GenreCount("History", 3),
                new GenreCount("Science", 3),
                new GenreCount("Self-Help", 1),
            }, _library.CountByGenre());
        }

        // ---- 4. oldest book ----
        [Fact]
        public void OldestBook_IsPrideAndPrejudice()
        {
            Book? oldest = _library.OldestBook();

            Assert.Equal("Pride and Prejudice", oldest!.Title);
            Assert.Equal(1813, oldest.Year);
        }

        [Fact]
        public void OldestBook_TieOnYear_PicksTitleAlphabetically()
        {
            var library = new LibraryCatalog(new[]
            {
                new Book("Zeta", "A", "G", 1900, true),
                new Book("Alpha", "B", "G", 1900, false),
            });

            Assert.Equal("Alpha", library.OldestBook()!.Title);
        }

        [Fact]
        public void OldestBook_EmptyLibrary_IsNull()
        {
            Assert.Null(new LibraryCatalog(Array.Empty<Book>()).OldestBook());
        }

        // ---- 5. published after a year, sorted by title ----
        [Fact]
        public void PublishedAfter2010_SortedByTitle()
        {
            Assert.Equal(
                new[] { "Atomic Habits", "Clean Architecture", "Homo Deus", "Sapiens", "The Clean Coder", "The Gene" },
                _library.PublishedAfter(2010).Select(b => b.Title));
        }

        [Fact]
        public void PublishedAfter_IsExclusiveOfTheYear()
        {
            Assert.DoesNotContain(_library.PublishedAfter(2011), b => b.Year == 2011);
            Assert.Empty(_library.PublishedAfter(2100));
        }

        // ---- 6. search by title ----
        [Theory]
        [InlineData("clean", 3)]
        [InlineData("CLEAN", 3)]
        [InlineData("  clean  ", 3)]   // trimmed
        [InlineData("history", 1)]
        [InlineData("xyz", 0)]
        public void SearchByTitle_IsCaseInsensitive(string keyword, int expected)
        {
            Assert.Equal(expected, _library.SearchByTitle(keyword).Count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SearchByTitle_BlankKeyword_ReturnsEmpty(string? keyword)
        {
            Assert.Empty(_library.SearchByTitle(keyword));
        }

        [Fact]
        public void Constructor_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new LibraryCatalog(null!));
        }

        [Fact]
        public void Books_IsReadOnly()
        {
            Assert.Throws<NotSupportedException>(() => ((IList<Book>)_library.Books).Clear());
        }

        [Fact]
        public void Demo_Run_PrintsAllQueries()
        {
            string output = ConsoleCapture.Run(LibraryDemo.Run);

            Assert.Contains("Library has 16 books.", output);
            Assert.Contains("George Orwell: 1984, Animal Farm", output);
            Assert.Contains("2) Available by 'robert c. martin': Clean Architecture, Clean Code", output);
            Assert.Contains("Programming  5", output);
            Assert.Contains("4) Oldest book: Pride and Prejudice (1813)", output);
            Assert.Contains("Atomic Habits (2018)", output);
            Assert.Contains("6) Title contains 'clean': Clean Architecture, Clean Code, The Clean Coder", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEverySectionInOrder()
        {
            string output = ConsoleCapture.Run(() => ExtensionsMiniProjects.Program.Main(Array.Empty<string>()));

            string[] headers =
            {
                "=== Task 2.14: Extension methods", "=== Task 2.14: Anonymous types",
                "=== Mini Q4", "=== Mini Q5", "=== Mini Q6",
            };
            int position = -1;
            foreach (string header in headers)
            {
                int next = output.IndexOf(header, StringComparison.Ordinal);
                Assert.True(next > position, $"Expected \"{header}\" after position {position}.");
                position = next;
            }
        }
    }
}
