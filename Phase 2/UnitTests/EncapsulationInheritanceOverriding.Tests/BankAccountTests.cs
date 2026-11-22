using System.Reflection;
using EncapsulationInheritanceOverriding.Encapsulation;

namespace EncapsulationInheritanceOverriding.Tests
{
    // Task 2.1 - encapsulation
    public class BankAccountTests
    {
        [Fact]
        public void Constructor_SetsHolderBalanceAndFirstHistoryEntry()
        {
            var account = new BankAccount("Asha", 1000m);

            Assert.Equal("Asha", account.AccountHolder);
            Assert.Equal(1000m, account.GetBalance());
            Assert.Equal("Opened account with 1,000.00", Assert.Single(account.History));
        }

        [Fact]
        public void Constructor_DefaultOpeningBalance_IsZero()
        {
            Assert.Equal(0m, new BankAccount("Asha").GetBalance());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_BlankHolder_Throws(string holder)
        {
            Assert.Throws<ArgumentException>(() => new BankAccount(holder));
        }

        [Fact]
        public void Constructor_NegativeOpeningBalance_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BankAccount("Asha", -1m));
        }

        [Fact]
        public void Deposit_Positive_IncreasesBalanceAndLogs()
        {
            var account = new BankAccount("Asha", 100m);

            Assert.True(account.Deposit(50m));
            Assert.Equal(150m, account.GetBalance());
            Assert.Equal("Deposited 50.00. Balance: 150.00", account.History[^1]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-25)]
        public void Deposit_ZeroOrNegative_IsRejectedAndLogged(int amount)
        {
            var account = new BankAccount("Asha", 100m);

            Assert.False(account.Deposit(amount));
            Assert.Equal(100m, account.GetBalance());
            Assert.StartsWith("Rejected deposit", account.History[^1]);
        }

        [Fact]
        public void Withdraw_WithinBalance_DecreasesBalanceAndLogs()
        {
            var account = new BankAccount("Asha", 100m);

            Assert.True(account.Withdraw(40m));
            Assert.Equal(60m, account.GetBalance());
            Assert.Equal("Withdrew 40.00. Balance: 60.00", account.History[^1]);
        }

        [Fact]
        public void Withdraw_ExactBalance_IsAllowed()
        {
            var account = new BankAccount("Asha", 100m);

            Assert.True(account.Withdraw(100m));
            Assert.Equal(0m, account.GetBalance());
        }

        [Fact]
        public void Withdraw_MoreThanBalance_IsRejected_NoOverdraw()
        {
            var account = new BankAccount("Asha", 100m);

            Assert.False(account.Withdraw(100.01m));
            Assert.Equal(100m, account.GetBalance());
            Assert.Equal("Rejected withdrawal of 100.01: insufficient funds (balance 100.00)", account.History[^1]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Withdraw_ZeroOrNegative_IsRejected(int amount)
        {
            var account = new BankAccount("Asha", 100m);

            Assert.False(account.Withdraw(amount));
            Assert.Contains("amount must be positive", account.History[^1]);
        }

        [Fact]
        public void History_LogsEveryOperation_IncludingRejected()
        {
            var account = new BankAccount("Asha", 1000m);
            account.Deposit(500m);
            account.Withdraw(200m);
            account.Withdraw(5000m);
            account.Deposit(-50m);

            Assert.Equal(5, account.History.Count);
        }

        [Fact]
        public void History_CannotBeModifiedFromOutside()
        {
            var account = new BankAccount("Asha");
            var asList = (IList<string>)account.History;

            Assert.Throws<NotSupportedException>(() => asList.Add("hacked"));
        }

        [Fact]
        public void Balance_IsAPrivateField()
        {
            FieldInfo? field = typeof(BankAccount).GetField("_balance", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.NotNull(field);
            Assert.True(field!.IsPrivate);
            Assert.Null(typeof(BankAccount).GetProperty("Balance"));
        }

        [Fact]
        public void PrintHistory_PrintsNumberedEntries()
        {
            var account = new BankAccount("Asha", 10m);
            account.Deposit(5m);

            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(account.PrintHistory));

            Assert.Equal("History for Asha:", lines[0]);
            Assert.Equal("  1. Opened account with 10.00", lines[1]);
            Assert.Equal("  2. Deposited 5.00. Balance: 15.00", lines[2]);
        }

        [Fact]
        public void Demo_Run_ShowsRejectedOverdrawAndHistory()
        {
            string output = ConsoleCapture.Run(BankAccountDemo.Run);

            Assert.Contains("Withdraw 5000     -> Rejected", output);
            Assert.Contains("Balance           =  1,300.00", output);
            Assert.Contains("4. Rejected withdrawal of 5,000.00: insufficient funds (balance 1,300.00)", output);
        }
    }
}
