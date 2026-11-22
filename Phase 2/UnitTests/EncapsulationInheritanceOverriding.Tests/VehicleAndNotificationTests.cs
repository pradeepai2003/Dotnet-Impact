using System.Reflection;
using EncapsulationInheritanceOverriding.Inheritance;
using EncapsulationInheritanceOverriding.Overriding;

namespace EncapsulationInheritanceOverriding.Tests
{
    // Task 2.2 - inheritance and constructor chaining
    public class VehicleTests
    {
        [Fact]
        public void ElectricCar_ConstructorChain_RunsTopToBottom()
        {
            string[] lines = ConsoleCapture.Lines(ConsoleCapture.Run(() => new ElectricCar("Tesla", "Model 3", 2024, 4, 75)));

            Assert.Equal("  [1] Vehicle constructor: 2024 Tesla Model 3", lines[0]);
            Assert.Equal("  [2] Car constructor: 4 doors", lines[1]);
            Assert.Equal("  [3] ElectricCar constructor: 75 kWh", lines[2]);
        }

        [Fact]
        public void ElectricCar_InheritsAllProperties()
        {
            ElectricCar? car = null;
            ConsoleCapture.Run(() => car = new ElectricCar("Tata", "Nexon EV", 2023, 5, 40.5));

            Assert.Equal("Tata", car!.Make);
            Assert.Equal("Nexon EV", car.Model);
            Assert.Equal(2023, car.Year);
            Assert.Equal(5, car.NumberOfDoors);
            Assert.Equal(40.5, car.BatteryCapacity);
            Assert.IsAssignableFrom<Car>(car);
            Assert.IsAssignableFrom<Vehicle>(car);
        }

        public static TheoryData<Func<Vehicle>, string> DisplayCases => new()
        {
            { () => new Vehicle("Force", "Traveller", 2020), "Vehicle: 2020 Force Traveller" },
            { () => new Car("Honda", "City", 2022, 4), "Car: 2022 Honda City, 4 doors" },
            { () => new Bike("Royal Enfield", "Classic 350", 2021, true), "Bike: 2021 Royal Enfield Classic 350, with sidecar" },
            { () => new Bike("Bajaj", "Pulsar", 2019, false), "Bike: 2019 Bajaj Pulsar, without sidecar" },
            { () => new ElectricCar("Tesla", "Model 3", 2024, 4, 75), "Electric car: 2024 Tesla Model 3, 4 doors, 75 kWh battery" },
        };

        [Theory]
        [MemberData(nameof(DisplayCases))]
        public void DisplayInfo_EachOverridePrintsItsOwnInfo(Func<Vehicle> create, string expected)
        {
            Vehicle vehicle = null!;
            ConsoleCapture.Run(() => vehicle = create());

            string output = ConsoleCapture.Run(vehicle.DisplayInfo);   // called through a Vehicle reference

            Assert.Equal(expected, output.Trim());
        }

        [Theory]
        [InlineData(1885)]
        [InlineData(3000)]
        public void Vehicle_InvalidYear_Throws(int year)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ConsoleCapture.Run(() => new Vehicle("A", "B", year)));
        }

        [Fact]
        public void Car_ZeroDoors_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ConsoleCapture.Run(() => new Car("A", "B", 2020, 0)));
        }

        [Fact]
        public void ElectricCar_ZeroBattery_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ConsoleCapture.Run(() => new ElectricCar("A", "B", 2020, 4, 0)));
        }

        [Fact]
        public void Demo_Run_ShowsChainAndOverrides()
        {
            string output = ConsoleCapture.Run(VehicleDemo.Run);

            Assert.Contains("[3] ElectricCar constructor: 75 kWh", output);
            Assert.Contains("Car: 2022 Honda City, 4 doors", output);
            Assert.Contains("Electric car: 2024 Tesla Model 3, 4 doors, 75 kWh battery", output);
        }
    }

    // Task 2.3 - virtual / override / sealed
    public class NotificationTests
    {
        [Fact]
        public void BaseNotification_Send()
        {
            Assert.Equal("[Generic] To admin: Hi", new Notification("admin").Send("Hi"));
        }

        [Fact]
        public void Email_Send_UsesSubject()
        {
            var email = new EmailNotification("a@b.com") { Subject = "Welcome" };

            Assert.Equal("[Email] To a@b.com, Subject \"Welcome\": Hi", email.Send("Hi"));
        }

        [Fact]
        public void Email_DefaultSubject()
        {
            Assert.Equal("(no subject)", new EmailNotification("a@b.com").Subject);
        }

        [Fact]
        public void Sms_ShortMessage_IsNotTruncated()
        {
            string message = new string('x', SmsNotification.MaxLength);

            Assert.Equal($"[SMS] To 9876543210: {message}", new SmsNotification("9876543210").Send(message));
        }

        [Fact]
        public void Sms_LongMessage_IsTruncatedTo160Characters()
        {
            string result = new SmsNotification("9876543210").Send(new string('x', 200));
            string body = result["[SMS] To 9876543210: ".Length..];

            Assert.Equal(160, body.Length);
            Assert.EndsWith("...", body);
        }

        [Fact]
        public void Push_Send()
        {
            Assert.Equal("[Push] To device device-1: Hi", new PushNotification("device-1").Send("Hi"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void BlankRecipient_Throws(string recipient)
        {
            Assert.Throws<ArgumentException>(() => new PushNotification(recipient));
        }

        [Fact]
        public void Send_ThroughBaseReference_UsesRuntimeType()
        {
            Notification notification = new SmsNotification("9876543210");

            Assert.StartsWith("[SMS]", notification.Send("Hi"));
        }

        [Fact]
        public void EmailSend_IsSealedOverride()
        {
            MethodInfo send = typeof(EmailNotification).GetMethod(nameof(EmailNotification.Send))!;

            Assert.True(send.IsVirtual);
            Assert.True(send.IsFinal);   // 'sealed' compiles to a final virtual method
            Assert.Equal(typeof(EmailNotification), send.DeclaringType);
        }

        [Fact]
        public void PriorityEmail_CannotOverride_ButCanCallSealedSend()
        {
            var priority = new PriorityEmailNotification("ops@x.com") { Subject = "Alert" };

            Assert.Equal(typeof(EmailNotification), typeof(PriorityEmailNotification).GetMethod("Send")!.DeclaringType);
            Assert.Equal("[Email] To ops@x.com, Subject \"Alert\": URGENT: Disk full", priority.SendUrgent("Disk full"));
        }

        [Fact]
        public void Demo_Run_PrintsEveryChannel()
        {
            string output = ConsoleCapture.Run(NotificationDemo.Run);

            Assert.Contains("[Generic] To admin", output);
            Assert.Contains("[Email] To asha@example.com", output);
            Assert.Contains("[SMS] To 9876543210", output);
            Assert.Contains("[Push] To device device-42", output);
            Assert.Contains("URGENT: Database is not responding", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => EncapsulationInheritanceOverriding.Program.Main(Array.Empty<string>()));

            int t1 = output.IndexOf("=== Task 2.1", StringComparison.Ordinal);
            int t2 = output.IndexOf("=== Task 2.2", StringComparison.Ordinal);
            int t3 = output.IndexOf("=== Task 2.3", StringComparison.Ordinal);

            Assert.True(t1 >= 0 && t1 < t2 && t2 < t3);
        }
    }
}
