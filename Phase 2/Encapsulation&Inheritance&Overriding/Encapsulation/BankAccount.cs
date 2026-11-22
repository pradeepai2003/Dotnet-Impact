// Task 2.1 - Encapsulation: private state, public behaviour with validation.
using System.Globalization;

namespace EncapsulationInheritanceOverriding.Encapsulation
{
    public class BankAccount
    {
        // PRIVATE: nothing outside this class can touch the balance or history directly.
        // The only way to change them is through Deposit/Withdraw, which validate first.
        private decimal _balance;
        private readonly List<string> _history = new();

        public string AccountHolder { get; }

        public BankAccount(string accountHolder, decimal openingBalance = 0m)
        {
            if (string.IsNullOrWhiteSpace(accountHolder))
                throw new ArgumentException("Account holder is required.", nameof(accountHolder));
            if (openingBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(openingBalance), openingBalance, "Opening balance cannot be negative.");

            AccountHolder = accountHolder;
            _balance = openingBalance;
            _history.Add($"Opened account with {Format(openingBalance)}");
        }

        // Read-only view: callers can look at the history but cannot add/remove entries.
        public IReadOnlyList<string> History => _history.AsReadOnly();

        public decimal GetBalance() => _balance;

        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                _history.Add($"Rejected deposit of {Format(amount)}: amount must be positive");
                return false;
            }

            _balance += amount;
            _history.Add($"Deposited {Format(amount)}. Balance: {Format(_balance)}");
            return true;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                _history.Add($"Rejected withdrawal of {Format(amount)}: amount must be positive");
                return false;
            }
            if (amount > _balance)
            {
                // No overdraw: the balance can never go below zero.
                _history.Add($"Rejected withdrawal of {Format(amount)}: insufficient funds (balance {Format(_balance)})");
                return false;
            }

            _balance -= amount;
            _history.Add($"Withdrew {Format(amount)}. Balance: {Format(_balance)}");
            return true;
        }

        public void PrintHistory()
        {
            Console.WriteLine($"History for {AccountHolder}:");
            for (int i = 0; i < _history.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {_history[i]}");
            }
        }

        internal static string Format(decimal amount) => amount.ToString("N2", CultureInfo.InvariantCulture);
    }

    public static class BankAccountDemo
    {
        public static void Run()
        {
            var account = new BankAccount("Asha", 1000m);

            Console.WriteLine($"Deposit 500       -> {Result(account.Deposit(500m))}");
            Console.WriteLine($"Withdraw 200      -> {Result(account.Withdraw(200m))}");
            Console.WriteLine($"Withdraw 5000     -> {Result(account.Withdraw(5000m))}   (overdraw)");
            Console.WriteLine($"Deposit -50       -> {Result(account.Deposit(-50m))}   (negative)");
            Console.WriteLine($"Balance           =  {BankAccount.Format(account.GetBalance())}");
            account.PrintHistory();

            // account._balance = 1_000_000;
            //   ERROR CS0122: 'BankAccount._balance' is inaccessible due to its protection level
            //   WHY: the field is private - encapsulation forces every change through validated methods.
        }

        private static string Result(bool ok) => ok ? "OK" : "Rejected";
    }
}
