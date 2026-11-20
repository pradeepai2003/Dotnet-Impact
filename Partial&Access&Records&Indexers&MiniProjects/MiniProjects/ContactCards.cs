// Mini Q3 - Contact cards: struct array + case-insensitive search.
namespace PartialAccessRecordsIndexers.MiniProjects
{
    public struct ContactCard
    {
        public string Name;
        public string Phone;
        public string Email;

        public ContactCard(string name, string phone, string email)
        {
            Name = name;
            Phone = phone;
            Email = email;
        }

        public override string ToString() => $"{Name,-15} {Phone,-12} {Email}";
    }

    public static class ContactCards
    {
        public static void Run()
        {
            ContactCard[] contacts =
            {
                new("Ravi Kumar",     "9876500001", "ravi@example.com"),
                new("SNEHA Iyer",     "9876500002", "sneha@example.com"),
                new("Mohammed Arif",  "9876500003", "arif@example.com"),
                new("Lakshmi Nair",   "9876500004", "lakshmi@example.com"),
                new("John D'Souza",   "9876500005", "john@example.com"),
            };

            foreach (string query in new[] { "sneha", "RAVI", "nair", "priya" })
            {
                ContactCard[] matches = Search(contacts, query);
                Console.WriteLine($"Search \"{query}\": {matches.Length} match(es)");
                foreach (ContactCard c in matches)
                    Console.WriteLine($"   {c}");
            }
        }

        // StringComparison.OrdinalIgnoreCase ignores upper/lower case without creating
        // new lowercase strings (better than name.ToLower().Contains(query.ToLower())).
        public static ContactCard[] Search(ContactCard[] contacts, string query) =>
            contacts.Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
