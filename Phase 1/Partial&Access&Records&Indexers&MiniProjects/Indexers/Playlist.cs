// Task 1.17 - Indexers: lets an object be used like an array -> playlist[0], playlist["title"].
namespace PartialAccessRecordsIndexers.Indexers
{
    public class Playlist
    {
        private readonly List<string> _songs = new();

        public string Name { get; }
        public int Count => _songs.Count;

        public Playlist(string name, params string[] songs)
        {
            Name = name;
            _songs.AddRange(songs);
        }

        // int indexer with bounds checking (get + set).
        public string this[int index]
        {
            get
            {
                CheckBounds(index);
                return _songs[index];
            }
            set
            {
                CheckBounds(index);
                _songs[index] = value;
            }
        }

        // string indexer: look up a song's position by title (case-insensitive). -1 if not found.
        public int this[string title] =>
            _songs.FindIndex(s => s.Equals(title, StringComparison.OrdinalIgnoreCase));

        private void CheckBounds(int index)
        {
            if (index < 0 || index >= _songs.Count)
                throw new IndexOutOfRangeException(
                    $"Index {index} is out of range. '{Name}' has {_songs.Count} songs (valid: 0-{_songs.Count - 1}).");
        }
    }

    public static class IndexerDemo
    {
        public static void Run()
        {
            var playlist = new Playlist("Road Trip", "Starlight", "Ocean Drive", "Golden Hour");

            Console.WriteLine($"playlist[0] = {playlist[0]}");
            playlist[1] = "Midnight Run";                         // set via indexer
            Console.WriteLine($"playlist[1] after update = {playlist[1]}");

            try
            {
                Console.WriteLine(playlist[5]);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Bounds check: {ex.Message}");
            }

            Console.WriteLine($"playlist[\"golden hour\"] -> position {playlist["golden hour"]}");
            Console.WriteLine($"playlist[\"Unknown\"]     -> position {playlist["Unknown"]}");
        }
    }
}
