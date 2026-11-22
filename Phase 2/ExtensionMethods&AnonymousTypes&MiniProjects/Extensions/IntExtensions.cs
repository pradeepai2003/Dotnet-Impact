// Task 2.14 - Extension method on int: 0-999 in words.
namespace ExtensionsMiniProjects.Extensions
{
    public static class IntExtensions
    {
        private static readonly string[] Ones =
        {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
            "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
        };

        private static readonly string[] Tens =
        {
            "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety",
        };

        // 7 -> "seven", 45 -> "forty-five", 105 -> "one hundred and five", 999 -> "nine hundred and ninety-nine"
        public static string ToWords(this int number)
        {
            if (number < 0 || number > 999)
                throw new ArgumentOutOfRangeException(nameof(number), number, "Only numbers from 0 to 999 are supported.");

            if (number < 20)
                return Ones[number];

            if (number < 100)
            {
                int tens = number / 10, units = number % 10;
                return units == 0 ? Tens[tens] : $"{Tens[tens]}-{Ones[units]}";
            }

            int hundreds = number / 100, remainder = number % 100;
            return remainder == 0
                ? $"{Ones[hundreds]} hundred"
                : $"{Ones[hundreds]} hundred and {remainder.ToWords()}";
        }
    }
}
