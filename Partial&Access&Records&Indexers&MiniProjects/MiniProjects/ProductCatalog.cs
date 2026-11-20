// Mini Q1 - Product Catalog grouped by category.
namespace PartialAccessRecordsIndexers.MiniProjects
{
    public enum Category { Electronics, Grocery, Clothing }

    public class Product
    {
        public string Name { get; }
        public decimal Price { get; }
        public Category Category { get; }

        public Product(string name, decimal price, Category category)
        {
            Name = name;
            Price = price;
            Category = category;
        }
    }

    public static class ProductCatalog
    {
        public static void Run()
        {
            var products = new List<Product>
            {
                new("Laptop",     65000m, Category.Electronics),
                new("Rice 5kg",     450m, Category.Grocery),
                new("T-Shirt",      799m, Category.Clothing),
                new("Headphones",  2499m, Category.Electronics),
                new("Olive Oil",    899m, Category.Grocery),
            };

            // GroupBy collects products with the same Category; OrderBy keeps enum order.
            foreach (var group in products.GroupBy(p => p.Category).OrderBy(g => g.Key))
            {
                Console.WriteLine($"{group.Key} ({group.Count()})");
                foreach (Product p in group.OrderBy(p => p.Name))
                {
                    Console.WriteLine($"   - {p.Name,-12} {p.Price,10:N2}");
                }
            }
        }
    }
}
