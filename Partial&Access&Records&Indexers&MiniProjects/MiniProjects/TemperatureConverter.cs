// Mini Q2 - Temperature converter using method overloading.
// Each unit is its own small type, so Convert(...) is OVERLOADED by parameter type:
// the compiler picks the right version based on what you pass in.
namespace PartialAccessRecordsIndexers.MiniProjects
{
    public readonly record struct Celsius(double Value);
    public readonly record struct Fahrenheit(double Value);
    public readonly record struct Kelvin(double Value);

    public static class TemperatureConverter
    {
        // Overload 1: Celsius -> Fahrenheit, Kelvin
        public static (Fahrenheit F, Kelvin K) Convert(Celsius c) =>
            (new Fahrenheit(c.Value * 9 / 5 + 32), new Kelvin(c.Value + 273.15));

        // Overload 2: Fahrenheit -> Celsius, Kelvin
        public static (Celsius C, Kelvin K) Convert(Fahrenheit f)
        {
            double c = (f.Value - 32) * 5 / 9;
            return (new Celsius(c), new Kelvin(c + 273.15));
        }

        // Overload 3: Kelvin -> Celsius, Fahrenheit
        public static (Celsius C, Fahrenheit F) Convert(Kelvin k)
        {
            double c = k.Value - 273.15;
            return (new Celsius(c), new Fahrenheit(c * 9 / 5 + 32));
        }

        // Entry point taking "value + unit" (e.g. 100, 'C') and routing to the right overload.
        public static void ConvertAndPrint(double value, char unit)
        {
            switch (char.ToUpperInvariant(unit))
            {
                case 'C':
                    var (f1, k1) = Convert(new Celsius(value));
                    Console.WriteLine($"{value,8:F2} °C = {f1.Value,8:F2} °F = {k1.Value,8:F2} K");
                    break;
                case 'F':
                    var (c2, k2) = Convert(new Fahrenheit(value));
                    Console.WriteLine($"{value,8:F2} °F = {c2.Value,8:F2} °C = {k2.Value,8:F2} K");
                    break;
                case 'K':
                    var (c3, f3) = Convert(new Kelvin(value));
                    Console.WriteLine($"{value,8:F2} K  = {c3.Value,8:F2} °C = {f3.Value,8:F2} °F");
                    break;
                default:
                    Console.WriteLine($"Unknown unit '{unit}'. Use C, F or K.");
                    break;
            }
        }

        public static void Run()
        {
            ConvertAndPrint(100, 'C');      // expect 212 °F, 373.15 K
            ConvertAndPrint(32, 'F');       // expect 0 °C, 273.15 K
            ConvertAndPrint(0, 'K');        // expect -273.15 °C, -459.67 °F
            ConvertAndPrint(-40, 'c');      // -40 is the same in °C and °F
            ConvertAndPrint(10, 'X');       // invalid unit
        }
    }
}
