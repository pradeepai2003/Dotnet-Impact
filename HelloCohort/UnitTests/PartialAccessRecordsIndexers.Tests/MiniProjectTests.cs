using PartialAccessRecordsIndexers.MiniProjects;

namespace PartialAccessRecordsIndexers.Tests
{
    // Mini Q1 - Product Catalog
    public class ProductCatalogTests
    {
        [Fact]
        public void Product_Constructor_SetsProperties()
        {
            var product = new Product("Laptop", 65000m, Category.Electronics);

            Assert.Equal("Laptop", product.Name);
            Assert.Equal(65000m, product.Price);
            Assert.Equal(Category.Electronics, product.Category);
        }

        [Fact]
        public void Run_GroupsProductsByCategoryInEnumOrder()
        {
            string output = ConsoleCapture.Run(ProductCatalog.Run);

            int electronics = output.IndexOf("Electronics (2)", StringComparison.Ordinal);
            int grocery = output.IndexOf("Grocery (2)", StringComparison.Ordinal);
            int clothing = output.IndexOf("Clothing (1)", StringComparison.Ordinal);

            Assert.True(electronics >= 0 && electronics < grocery && grocery < clothing);
        }

        [Fact]
        public void Run_ListsEachProductUnderItsOwnCategory()
        {
            string output = ConsoleCapture.Run(ProductCatalog.Run);

            int electronics = output.IndexOf("Electronics", StringComparison.Ordinal);
            int grocery = output.IndexOf("Grocery", StringComparison.Ordinal);
            int clothing = output.IndexOf("Clothing", StringComparison.Ordinal);

            int headphones = output.IndexOf("Headphones", StringComparison.Ordinal);
            int laptop = output.IndexOf("Laptop", StringComparison.Ordinal);
            int oliveOil = output.IndexOf("Olive Oil", StringComparison.Ordinal);
            int rice = output.IndexOf("Rice 5kg", StringComparison.Ordinal);
            int tshirt = output.IndexOf("T-Shirt", StringComparison.Ordinal);

            Assert.InRange(headphones, electronics, grocery);
            Assert.InRange(laptop, electronics, grocery);
            Assert.True(headphones < laptop, "Products are sorted by name inside a group.");
            Assert.InRange(oliveOil, grocery, clothing);
            Assert.InRange(rice, grocery, clothing);
            Assert.True(tshirt > clothing);
            Assert.Contains("65,000.00", output);
        }
    }

    // Mini Q2 - Temperature Converter (method overloading)
    public class TemperatureConverterTests
    {
        [Fact]
        public void Convert_Celsius_ReturnsFahrenheitAndKelvin()
        {
            var (f, k) = TemperatureConverter.Convert(new Celsius(100));

            Assert.Equal(212, f.Value, precision: 2);
            Assert.Equal(373.15, k.Value, precision: 2);
        }

        [Fact]
        public void Convert_Fahrenheit_ReturnsCelsiusAndKelvin()
        {
            var (c, k) = TemperatureConverter.Convert(new Fahrenheit(32));

            Assert.Equal(0, c.Value, precision: 2);
            Assert.Equal(273.15, k.Value, precision: 2);
        }

        [Fact]
        public void Convert_Kelvin_ReturnsCelsiusAndFahrenheit()
        {
            var (c, f) = TemperatureConverter.Convert(new Kelvin(0));

            Assert.Equal(-273.15, c.Value, precision: 2);
            Assert.Equal(-459.67, f.Value, precision: 2);
        }

        [Fact]
        public void Convert_MinusForty_IsSameInCelsiusAndFahrenheit()
        {
            var (f, _) = TemperatureConverter.Convert(new Celsius(-40));

            Assert.Equal(-40, f.Value, precision: 2);
        }

        [Fact]
        public void Convert_RoundTrip_ReturnsOriginalValue()
        {
            var (f, _) = TemperatureConverter.Convert(new Celsius(37));
            var (c, _) = TemperatureConverter.Convert(f);

            Assert.Equal(37, c.Value, precision: 6);
        }

        [Theory]
        [InlineData(100, 'C', "212.00 °F")]
        [InlineData(100, 'c', "373.15 K")]
        [InlineData(32, 'F', "0.00 °C")]
        [InlineData(212, 'f', "373.15 K")]
        [InlineData(0, 'K', "-273.15 °C")]
        [InlineData(0, 'k', "-459.67 °F")]
        public void ConvertAndPrint_EachUnit_PrintsOtherTwoUnits(double value, char unit, string expected)
        {
            string output = ConsoleCapture.Run(() => TemperatureConverter.ConvertAndPrint(value, unit));

            Assert.Contains(expected, output);
        }

        [Fact]
        public void ConvertAndPrint_UnknownUnit_PrintsError()
        {
            string output = ConsoleCapture.Run(() => TemperatureConverter.ConvertAndPrint(10, 'X'));

            Assert.Equal("Unknown unit 'X'. Use C, F or K.", output.Trim());
        }

        [Fact]
        public void Run_ConvertsAllSamples()
        {
            string output = ConsoleCapture.Run(TemperatureConverter.Run);

            Assert.Contains("212.00 °F", output);
            Assert.Contains("273.15 K", output);
            Assert.Contains("-459.67 °F", output);
            Assert.Contains("Unknown unit 'X'", output);
        }
    }

    // Mini Q3 - Contact Cards
    public class ContactCardTests
    {
        private static ContactCard[] Contacts() => new[]
        {
            new ContactCard("Ravi Kumar", "9876500001", "ravi@example.com"),
            new ContactCard("SNEHA Iyer", "9876500002", "sneha@example.com"),
            new ContactCard("Mohammed Arif", "9876500003", "arif@example.com"),
            new ContactCard("Lakshmi Nair", "9876500004", "lakshmi@example.com"),
            new ContactCard("John D'Souza", "9876500005", "john@example.com"),
        };

        [Fact]
        public void ContactCard_Constructor_SetsFields()
        {
            var card = new ContactCard("Ravi Kumar", "9876500001", "ravi@example.com");

            Assert.Equal("Ravi Kumar", card.Name);
            Assert.Equal("9876500001", card.Phone);
            Assert.Equal("ravi@example.com", card.Email);
            Assert.True(typeof(ContactCard).IsValueType);
        }

        [Fact]
        public void ContactCard_ToString_UsesFixedWidthColumns()
        {
            var card = new ContactCard("Ravi Kumar", "9876500001", "ravi@example.com");

            Assert.Equal("Ravi Kumar      9876500001   ravi@example.com", card.ToString());
        }

        [Theory]
        [InlineData("sneha", "SNEHA Iyer")]
        [InlineData("RAVI", "Ravi Kumar")]
        [InlineData("nair", "Lakshmi Nair")]
        [InlineData("mOhAmMeD", "Mohammed Arif")]
        public void Search_IsCaseInsensitive(string query, string expectedName)
        {
            ContactCard[] matches = ContactCards.Search(Contacts(), query);

            Assert.Single(matches);
            Assert.Equal(expectedName, matches[0].Name);
        }

        [Fact]
        public void Search_NoMatch_ReturnsEmpty()
        {
            Assert.Empty(ContactCards.Search(Contacts(), "priya"));
        }

        [Fact]
        public void Search_PartialText_CanMatchSeveral()
        {
            // "a" appears in every name.
            Assert.Equal(5, ContactCards.Search(Contacts(), "A").Length);
        }

        [Fact]
        public void Run_PrintsResultsForEachQuery()
        {
            string output = ConsoleCapture.Run(ContactCards.Run);

            Assert.Contains("Search \"sneha\": 1 match(es)", output);
            Assert.Contains("SNEHA Iyer", output);
            Assert.Contains("Search \"RAVI\": 1 match(es)", output);
            Assert.Contains("Search \"nair\": 1 match(es)", output);
            Assert.Contains("Search \"priya\": 0 match(es)", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEverySectionInOrder()
        {
            string output = ConsoleCapture.Run(() => PartialAccessRecordsIndexers.Program.Main(Array.Empty<string>()));

            string[] expectedInOrder =
            {
                "=== Task 1.17a: Partial class & partial method ===",
                "=== Task 1.17b: Access-modifier matrix ===",
                "=== Task 1.17c: Records ===",
                "=== Task 1.17d: Indexers ===",
                "=== Mini Q1: Product Catalog ===",
                "=== Mini Q2: Temperature Converter ===",
                "=== Mini Q3: Contact Cards ===",
                "Search \"priya\": 0 match(es)",
            };

            int position = -1;
            foreach (string expected in expectedInOrder)
            {
                int next = output.IndexOf(expected, StringComparison.Ordinal);
                Assert.True(next > position, $"Expected \"{expected}\" after position {position}.");
                position = next;
            }
        }
    }
}
