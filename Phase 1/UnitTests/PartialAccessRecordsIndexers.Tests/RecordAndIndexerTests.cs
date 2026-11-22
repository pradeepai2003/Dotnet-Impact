using PartialAccessRecordsIndexers.Indexers;
using PartialAccessRecordsIndexers.Records;

namespace PartialAccessRecordsIndexers.Tests
{
    // Task 1.17c - records
    public class RecordTests
    {
        private static Address Home() => new("12 MG Road", "Bengaluru", "560001");

        [Fact]
        public void Record_SameValues_AreEqual()
        {
            Address a = Home(), b = Home();

            Assert.True(a == b);
            Assert.Equal(a, b);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.False(ReferenceEquals(a, b));
        }

        [Fact]
        public void Record_DifferentValues_AreNotEqual()
        {
            Assert.True(Home() != new Address("12 MG Road", "Bengaluru", "560002"));
        }

        [Fact]
        public void With_CreatesChangedCopy_AndLeavesOriginalUnchanged()
        {
            Address home = Home();

            Address office = home with { Street = "45 Brigade Road", Pincode = "560025" };

            Assert.Equal("45 Brigade Road", office.Street);
            Assert.Equal("Bengaluru", office.City);
            Assert.Equal("560025", office.Pincode);
            Assert.Equal("12 MG Road", home.Street);
            Assert.NotEqual(home, office);
        }

        [Fact]
        public void Record_ToString_ListsAllProperties()
        {
            Assert.Equal("Address { Street = 12 MG Road, City = Bengaluru, Pincode = 560001 }", Home().ToString());
        }

        [Fact]
        public void Record_CanBeDeconstructed()
        {
            var (street, city, pincode) = Home();

            Assert.Equal(("12 MG Road", "Bengaluru", "560001"), (street, city, pincode));
        }

        [Fact]
        public void Class_SameValues_AreNotEqualByReference()
        {
            var c1 = new AddressClass { Street = "12 MG Road", City = "Bengaluru", Pincode = "560001" };
            var c2 = new AddressClass { Street = "12 MG Road", City = "Bengaluru", Pincode = "560001" };

            Assert.False(c1 == c2);
            Assert.Equal(c1.City, c2.City);
        }

        [Fact]
        public void Class_DefaultValues_AreEmpty()
        {
            var empty = new AddressClass();

            Assert.Equal("", empty.Street);
            Assert.Equal("", empty.City);
            Assert.Equal("", empty.Pincode);
        }

        [Fact]
        public void RecordDemo_Run_PrintsEqualityAndWithResults()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(RecordDemo.Run));

            Assert.Contains("record: home1 == home2 ? True", lines);
            Assert.Contains("record: same object?    False", lines);
            Assert.Contains("class:  c1 == c2 ? False", lines);
            Assert.Contains("with copy = Address { Street = 45 Brigade Road, City = Bengaluru, Pincode = 560025 }", lines);
            Assert.Contains("original  = Address { Street = 12 MG Road, City = Bengaluru, Pincode = 560001 }", lines);
            Assert.Contains("home1 == office ? False", lines);
        }
    }

    // Task 1.17d - indexers
    public class PlaylistTests
    {
        private static Playlist Sample() => new("Road Trip", "Starlight", "Ocean Drive", "Golden Hour");

        [Fact]
        public void Constructor_SetsNameAndCount()
        {
            Playlist playlist = Sample();

            Assert.Equal("Road Trip", playlist.Name);
            Assert.Equal(3, playlist.Count);
        }

        [Fact]
        public void IntIndexer_Get_ReturnsSong()
        {
            Assert.Equal("Starlight", Sample()[0]);
            Assert.Equal("Golden Hour", Sample()[2]);
        }

        [Fact]
        public void IntIndexer_Set_ReplacesSong()
        {
            Playlist playlist = Sample();

            playlist[1] = "Midnight Run";

            Assert.Equal("Midnight Run", playlist[1]);
            Assert.Equal(3, playlist.Count);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        [InlineData(100)]
        public void IntIndexer_Get_OutOfRange_Throws(int index)
        {
            var ex = Assert.Throws<IndexOutOfRangeException>(() => Sample()[index]);

            Assert.Contains($"Index {index} is out of range", ex.Message);
            Assert.Contains("(valid: 0-2)", ex.Message);
        }

        [Fact]
        public void IntIndexer_Set_OutOfRange_Throws()
        {
            Playlist playlist = Sample();

            Assert.Throws<IndexOutOfRangeException>(() => playlist[5] = "Nope");
        }

        [Fact]
        public void IntIndexer_EmptyPlaylist_Throws()
        {
            var empty = new Playlist("Empty");

            Assert.Equal(0, empty.Count);
            Assert.Throws<IndexOutOfRangeException>(() => empty[0]);
        }

        [Theory]
        [InlineData("Starlight", 0)]
        [InlineData("golden hour", 2)]
        [InlineData("OCEAN DRIVE", 1)]
        [InlineData("Unknown", -1)]
        public void StringIndexer_FindsPositionIgnoringCase(string title, int expected)
        {
            Assert.Equal(expected, Sample()[title]);
        }

        [Fact]
        public void IndexerDemo_Run_PrintsAllIndexerCases()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(IndexerDemo.Run));

            Assert.Contains("playlist[0] = Starlight", lines);
            Assert.Contains("playlist[1] after update = Midnight Run", lines);
            Assert.Contains("Bounds check: Index 5 is out of range. 'Road Trip' has 3 songs (valid: 0-2).", lines);
            Assert.Contains("playlist[\"golden hour\"] -> position 2", lines);
            Assert.Contains("playlist[\"Unknown\"]     -> position -1", lines);
        }
    }
}
