using PartialAccessRecordsIndexers.Partial;
using PartialAccessRecordsIndexers.AccessModifiers;
using PartialAccessRecordsIndexers.Records;
using PartialAccessRecordsIndexers.Indexers;
using PartialAccessRecordsIndexers.MiniProjects;

namespace PartialAccessRecordsIndexers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;   // so the ° symbol prints correctly

            Console.WriteLine("=== Task 1.17a: Partial class & partial method ===");
            PartialDemo.Run();

            Console.WriteLine("\n=== Task 1.17b: Access-modifier matrix ===");
            AccessDemo.Run();

            Console.WriteLine("\n=== Task 1.17c: Records ===");
            RecordDemo.Run();

            Console.WriteLine("\n=== Task 1.17d: Indexers ===");
            IndexerDemo.Run();

            Console.WriteLine("\n=== Mini Q1: Product Catalog ===");
            ProductCatalog.Run();

            Console.WriteLine("\n=== Mini Q2: Temperature Converter ===");
            TemperatureConverter.Run();

            Console.WriteLine("\n=== Mini Q3: Contact Cards ===");
            ContactCards.Run();
        }
    }
}
