using System.Collections;
using System.Reflection;
using GenericsYieldLinq.Generics;
using GenericsYieldLinq.Yield;

namespace GenericsYieldLinq.Tests
{
    // Task 2.11 - generic repository
    public class RepositoryTests
    {
        [Fact]
        public void SameRepository_WorksForStudentAndProduct()
        {
            var students = new Repository<Student>();
            var products = new Repository<Product>();

            students.Add(new Student { Id = 1, Name = "Asha", Grade = 8 });
            products.Add(new Product { Id = 1, Name = "Laptop", Price = 65000m });

            Assert.Equal("Asha", students.GetById(1)!.Name);
            Assert.Equal(65000m, products.GetById(1)!.Price);
        }

        [Fact]
        public void Add_ThenGetAll_ReturnsItemsInOrder()
        {
            var repo = new Repository<Student>();
            repo.Add(new Student { Id = 1, Name = "A" });
            repo.Add(new Student { Id = 2, Name = "B" });

            Assert.Equal(2, repo.Count);
            Assert.Equal(new[] { "A", "B" }, repo.GetAll().Select(s => s.Name));
        }

        [Fact]
        public void Add_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Repository<Student>().Add(null!));
        }

        [Fact]
        public void Add_DuplicateId_Throws()
        {
            var repo = new Repository<Product>();
            repo.Add(new Product { Id = 10 });

            var ex = Assert.Throws<InvalidOperationException>(() => repo.Add(new Product { Id = 10 }));
            Assert.Equal("Product with Id 10 already exists.", ex.Message);
        }

        [Fact]
        public void Update_Existing_ReplacesItem()
        {
            var repo = new Repository<Student>();
            repo.Add(new Student { Id = 1, Name = "Old" });

            Assert.True(repo.Update(new Student { Id = 1, Name = "New" }));
            Assert.Equal("New", repo.GetById(1)!.Name);
            Assert.Equal(1, repo.Count);
        }

        [Fact]
        public void Update_Missing_ReturnsFalse()
        {
            Assert.False(new Repository<Student>().Update(new Student { Id = 99 }));
        }

        [Fact]
        public void Update_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Repository<Student>().Update(null!));
        }

        [Fact]
        public void Delete_ExistingAndMissing()
        {
            var repo = new Repository<Student>();
            repo.Add(new Student { Id = 1 });

            Assert.True(repo.Delete(1));
            Assert.False(repo.Delete(1));
            Assert.Equal(0, repo.Count);
        }

        [Fact]
        public void GetById_Missing_ReturnsNull()
        {
            Assert.Null(new Repository<Product>().GetById(5));
        }

        [Fact]
        public void CreateNew_UsesParameterlessConstructor()
        {
            Student created = new Repository<Student>().CreateNew(7);

            Assert.Equal(7, created.Id);
            Assert.Equal("", created.Name);
        }

        [Fact]
        public void GetAll_IsReadOnly()
        {
            var all = (IList<Student>)new Repository<Student>().GetAll();

            Assert.Throws<NotSupportedException>(() => all.Add(new Student()));
        }

        [Fact]
        public void Constraint_IsClassAndNew()
        {
            GenericParameterAttributes attributes = typeof(Repository<>).GetGenericArguments()[0].GenericParameterAttributes;

            Assert.True(attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint));
            Assert.True(attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint));
        }

        [Fact]
        public void Models_ToString()
        {
            Assert.Equal("Student #1 Asha (grade 8)", new Student { Id = 1, Name = "Asha", Grade = 8 }.ToString());
            Assert.Equal("Product #2 Mouse (799.00)", new Product { Id = 2, Name = "Mouse", Price = 799m }.ToString());
        }

        [Fact]
        public void RepositoryDemo_Run()
        {
            string output = ConsoleCapture.Run(RepositoryDemo.Run);

            Assert.Contains("Student #2 Ravi Kumar (grade 10)", output);
            Assert.Contains("Student #3 Meena (grade 9)", output);
            Assert.DoesNotContain("Student #1", output);
            Assert.Contains("Update missing Id 99 -> False", output);
            Assert.Contains("Duplicate add rejected: Product with Id 10 already exists.", output);
        }
    }

    // Task 2.12 - yield
    public class IteratorTests
    {
        [Theory]
        [InlineData(10, new[] { 2, 4, 6, 8, 10 })]
        [InlineData(9, new[] { 2, 4, 6, 8 })]
        [InlineData(2, new[] { 2 })]
        [InlineData(1, new int[0])]
        [InlineData(0, new int[0])]
        public void GetEvenNumbers_ReturnsEvensUpToMax(int max, int[] expected)
        {
            Assert.Equal(expected, NumberSequences.GetEvenNumbers(max));
        }

        [Fact]
        public void GetEvenNumbers_NegativeMax_ThrowsImmediately()
        {
            // Throws at the CALL, before any enumeration, because validation happens outside the iterator.
            Assert.Throws<ArgumentOutOfRangeException>(() => NumberSequences.GetEvenNumbers(-1));
        }

        [Fact]
        public void GetEvenNumbers_IsLazy_NothingProducedUntilEnumerated()
        {
            var produced = new List<int>();

            IEnumerable<int> sequence = NumberSequences.GetEvenNumbers(1000, produced.Add);

            Assert.Empty(produced);                       // not started yet
            Assert.Equal(new[] { 2, 4 }, sequence.Take(2));
            Assert.Equal(new[] { 2, 4 }, produced);       // only what was asked for
        }

        [Fact]
        public void GetEvenNumbers_ForeachWithBreak_StopsProducing()
        {
            var produced = new List<int>();

            foreach (int n in NumberSequences.GetEvenNumbers(100, produced.Add))
            {
                if (n == 6) break;
            }

            Assert.Equal(new[] { 2, 4, 6 }, produced);
        }

        [Fact]
        public void BookCollection_YieldsAlphabeticallyIgnoringCase()
        {
            var books = new BookCollection
            {
                new Book("zebra", "A"),
                new Book("Apple", "B"),
                new Book("mango", "C"),
            };

            Assert.Equal(new[] { "Apple", "mango", "zebra" }, books.Select(b => b.Title));
            Assert.Equal(3, books.Count);
        }

        [Fact]
        public void BookCollection_NonGenericEnumerator_Works()
        {
            IEnumerable books = new BookCollection { new Book("B", "x"), new Book("A", "y") };

            var titles = new List<string>();
            foreach (object item in books) titles.Add(((Book)item).Title);

            Assert.Equal(new[] { "A", "B" }, titles);
        }

        [Fact]
        public void BookCollection_AddNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BookCollection().Add(null!));
        }

        [Fact]
        public void IteratorDemo_Run_ShowsLazyProduction()
        {
            string output = ConsoleCapture.Run(IteratorDemo.Run);

            Assert.Contains("produced 6", output);
            Assert.DoesNotContain("produced 8", output);
            Assert.True(output.IndexOf("Clean Code", StringComparison.Ordinal) < output.IndexOf("Design Patterns", StringComparison.Ordinal));
            Assert.True(output.IndexOf("refactoring", StringComparison.Ordinal) < output.IndexOf("The Pragmatic Programmer", StringComparison.Ordinal));
        }
    }
}
