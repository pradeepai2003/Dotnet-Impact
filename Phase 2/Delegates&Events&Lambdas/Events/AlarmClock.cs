// Task 2.9 - Events with custom EventArgs (publisher / subscriber).
using System.Globalization;

namespace DelegatesEventsLambdas.Events
{
    // Custom event data: subscribers receive the alarm time.
    public class AlarmEventArgs : EventArgs
    {
        public DateTime AlarmTime { get; }

        public AlarmEventArgs(DateTime alarmTime) => AlarmTime = alarmTime;

        public string TimeText => AlarmTime.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    // PUBLISHER: raises the event, knows nothing about who listens.
    public class AlarmClock
    {
        // 'event' = a delegate that outsiders can only += / -= (they can't invoke or reset it).
        public event EventHandler<AlarmEventArgs>? OnAlarmRing;

        public string Label { get; }

        public AlarmClock(string label) => Label = label;

        public int SubscriberCount => OnAlarmRing?.GetInvocationList().Length ?? 0;

        public void Ring(DateTime time)
        {
            var args = new AlarmEventArgs(time);
            Console.WriteLine($"Alarm '{Label}' ringing at {args.TimeText}");
            OnAlarmRing?.Invoke(this, args);   // ?. = do nothing when there are no subscribers
        }
    }

    // SUBSCRIBER 1
    public class Person
    {
        public string Name { get; }
        public DateTime? WokeUpAt { get; private set; }

        public Person(string name) => Name = name;

        public void WakeUp(object? sender, AlarmEventArgs e)
        {
            WokeUpAt = e.AlarmTime;
            Console.WriteLine($"  {Name} wakes up at {e.TimeText}");
        }
    }

    // SUBSCRIBER 2
    public class CoffeeMachine
    {
        public int CupsBrewed { get; private set; }
        public DateTime? LastBrewedAt { get; private set; }

        public void StartBrewing(object? sender, AlarmEventArgs e)
        {
            CupsBrewed++;
            LastBrewedAt = e.AlarmTime;
            Console.WriteLine($"  Coffee machine starts brewing at {e.TimeText} (cup #{CupsBrewed})");
        }
    }

    public static class AlarmDemo
    {
        public static void Run()
        {
            var clock = new AlarmClock("Morning");
            var person = new Person("Pradeep");
            var coffee = new CoffeeMachine();

            clock.OnAlarmRing += person.WakeUp;
            clock.OnAlarmRing += coffee.StartBrewing;
            Console.WriteLine($"Subscribers: {clock.SubscriberCount}");

            clock.Ring(new DateTime(2026, 11, 23, 6, 30, 0));

            // Unsubscribe the person: only the coffee machine reacts now.
            clock.OnAlarmRing -= person.WakeUp;
            Console.WriteLine($"Subscribers after unsubscribe: {clock.SubscriberCount}");
            clock.Ring(new DateTime(2026, 11, 23, 6, 40, 0));

            // clock.OnAlarmRing = null;          // ERROR CS0070: only += / -= allowed outside AlarmClock
            // clock.OnAlarmRing(clock, args);    // ERROR CS0070: only the publisher can raise the event
        }
    }
}
