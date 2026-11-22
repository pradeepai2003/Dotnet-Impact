// Task 2.7 - Operator overloading.
using System.Globalization;

namespace AbstractInterfacesPolymorphism.Operators
{
    public readonly struct Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required.", nameof(currency));
            Amount = amount;
            Currency = currency.Trim().ToUpperInvariant();
        }

        // + : only the same currency can be added.
        public static Money operator +(Money left, Money right)
        {
            EnsureSameCurrency(left, right, "add");
            return new Money(left.Amount + right.Amount, left.Currency);
        }

        // == and != MUST be overloaded together (otherwise CS0216).
        public static bool operator ==(Money left, Money right) => left.Equals(right);
        public static bool operator !=(Money left, Money right) => !left.Equals(right);

        // > and < MUST also be overloaded as a pair.
        public static bool operator >(Money left, Money right)
        {
            EnsureSameCurrency(left, right, "compare");
            return left.Amount > right.Amount;
        }

        public static bool operator <(Money left, Money right)
        {
            EnsureSameCurrency(left, right, "compare");
            return left.Amount < right.Amount;
        }

        // Overriding == also means overriding Equals and GetHashCode (otherwise warnings CS0660/CS0661).
        public bool Equals(Money other) => Amount == other.Amount && Currency == other.Currency;
        public override bool Equals(object? obj) => obj is Money other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        public override string ToString() => $"{Amount.ToString("N2", CultureInfo.InvariantCulture)} {Currency}";

        private static void EnsureSameCurrency(Money left, Money right, string operation)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException($"Cannot {operation} {left.Currency} and {right.Currency}.");
        }
    }

    public static class MoneyDemo
    {
        public static void Run()
        {
            var a = new Money(100m, "INR");
            var b = new Money(250.50m, "inr");   // normalised to INR
            var usd = new Money(10m, "USD");

            Console.WriteLine($"{a} + {b} = {a + b}");
            Console.WriteLine($"{a} == {new Money(100m, "INR")} ? {a == new Money(100m, "INR")}");
            Console.WriteLine($"{a} != {b} ? {a != b}");
            Console.WriteLine($"{a} > {b} ? {a > b}");
            Console.WriteLine($"{a} < {b} ? {a < b}");

            try
            {
                Money bad = a + usd;
                Console.WriteLine(bad);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"{a} + {usd} -> {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
