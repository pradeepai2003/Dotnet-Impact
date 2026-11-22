// Task 2.2 - Inheritance, constructor chaining with : base(...), and overriding.
using System.Globalization;

namespace EncapsulationInheritanceOverriding.Inheritance
{
    public class Vehicle
    {
        public string Make { get; }
        public string Model { get; }
        public int Year { get; }

        public Vehicle(string make, string model, int year)
        {
            if (year < 1886 || year > DateTime.Today.Year + 1)
                throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 1886 and next year.");

            Make = make;
            Model = model;
            Year = year;
            Console.WriteLine($"  [1] Vehicle constructor: {year} {make} {model}");
        }

        // protected: visible to derived classes, hidden from everyone else.
        protected string BasicInfo => $"{Year} {Make} {Model}";

        // virtual: derived classes MAY replace this behaviour with 'override'.
        public virtual void DisplayInfo() => Console.WriteLine($"Vehicle: {BasicInfo}");
    }

    public class Car : Vehicle
    {
        public int NumberOfDoors { get; }

        // ': base(...)' runs the Vehicle constructor FIRST, then this body.
        public Car(string make, string model, int year, int numberOfDoors)
            : base(make, model, year)
        {
            if (numberOfDoors <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfDoors), numberOfDoors, "A car needs at least one door.");

            NumberOfDoors = numberOfDoors;
            Console.WriteLine($"  [2] Car constructor: {numberOfDoors} doors");
        }

        public override void DisplayInfo() => Console.WriteLine($"Car: {BasicInfo}, {NumberOfDoors} doors");
    }

    public class Bike : Vehicle
    {
        public bool HasSidecar { get; }

        public Bike(string make, string model, int year, bool hasSidecar)
            : base(make, model, year)
        {
            HasSidecar = hasSidecar;
            Console.WriteLine($"  [2] Bike constructor: sidecar = {hasSidecar}");
        }

        public override void DisplayInfo() =>
            Console.WriteLine($"Bike: {BasicInfo}, {(HasSidecar ? "with" : "without")} sidecar");
    }

    // Three levels: ElectricCar -> Car -> Vehicle.
    public class ElectricCar : Car
    {
        public double BatteryCapacity { get; }   // kWh

        public ElectricCar(string make, string model, int year, int numberOfDoors, double batteryCapacity)
            : base(make, model, year, numberOfDoors)
        {
            if (batteryCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(batteryCapacity), batteryCapacity, "Battery capacity must be positive.");

            BatteryCapacity = batteryCapacity;
            Console.WriteLine($"  [3] ElectricCar constructor: {Kwh} kWh");
        }

        private string Kwh => BatteryCapacity.ToString(CultureInfo.InvariantCulture);

        public override void DisplayInfo() =>
            Console.WriteLine($"Electric car: {BasicInfo}, {NumberOfDoors} doors, {Kwh} kWh battery");
    }

    public static class VehicleDemo
    {
        public static void Run()
        {
            Console.WriteLine("Creating an ElectricCar (constructors run top-to-bottom):");
            var tesla = new ElectricCar("Tesla", "Model 3", 2024, 4, 75);

            Console.WriteLine("Creating a Car and a Bike:");
            var car = new Car("Honda", "City", 2022, 4);
            var bike = new Bike("Royal Enfield", "Classic 350", 2021, true);

            Console.WriteLine("DisplayInfo() through Vehicle references (each override prints its own info):");
            List<Vehicle> garage = new() { car, bike, tesla };
            foreach (Vehicle vehicle in garage)
            {
                vehicle.DisplayInfo();
            }
        }
    }
}
