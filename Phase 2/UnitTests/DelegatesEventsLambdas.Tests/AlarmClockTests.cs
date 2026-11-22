using DelegatesEventsLambdas.Events;

namespace DelegatesEventsLambdas.Tests
{
    // Task 2.9 - events
    public class AlarmClockTests
    {
        private static readonly DateTime SixThirty = new(2026, 11, 23, 6, 30, 0);

        [Fact]
        public void Ring_NotifiesBothSubscribersWithTheTime()
        {
            var clock = new AlarmClock("Morning");
            var person = new Person("Pradeep");
            var coffee = new CoffeeMachine();
            clock.OnAlarmRing += person.WakeUp;
            clock.OnAlarmRing += coffee.StartBrewing;

            string output = ConsoleCapture.Run(() => clock.Ring(SixThirty));

            Assert.Equal(SixThirty, person.WokeUpAt);
            Assert.Equal(SixThirty, coffee.LastBrewedAt);
            Assert.Equal(1, coffee.CupsBrewed);
            Assert.Contains("Alarm 'Morning' ringing at 06:30", output);
            Assert.Contains("Pradeep wakes up at 06:30", output);
            Assert.Contains("Coffee machine starts brewing at 06:30 (cup #1)", output);
        }

        [Fact]
        public void Ring_PassesClockAsSenderAndCustomArgs()
        {
            var clock = new AlarmClock("Test");
            object? receivedSender = null;
            AlarmEventArgs? receivedArgs = null;
            clock.OnAlarmRing += (sender, e) => { receivedSender = sender; receivedArgs = e; };

            ConsoleCapture.Run(() => clock.Ring(SixThirty));

            Assert.Same(clock, receivedSender);
            Assert.Equal(SixThirty, receivedArgs!.AlarmTime);
            Assert.Equal("06:30", receivedArgs.TimeText);
        }

        [Fact]
        public void Ring_WithNoSubscribers_DoesNotThrow()
        {
            var clock = new AlarmClock("Silent");

            string output = ConsoleCapture.Run(() => clock.Ring(SixThirty));

            Assert.Equal(0, clock.SubscriberCount);
            Assert.Contains("ringing at 06:30", output);
        }

        [Fact]
        public void Unsubscribe_StopsNotifications()
        {
            var clock = new AlarmClock("Morning");
            var person = new Person("Pradeep");
            var coffee = new CoffeeMachine();
            clock.OnAlarmRing += person.WakeUp;
            clock.OnAlarmRing += coffee.StartBrewing;

            clock.OnAlarmRing -= person.WakeUp;
            ConsoleCapture.Run(() => clock.Ring(SixThirty));

            Assert.Equal(1, clock.SubscriberCount);
            Assert.Null(person.WokeUpAt);
            Assert.Equal(1, coffee.CupsBrewed);
        }

        [Fact]
        public void EveryRing_BrewsAnotherCup()
        {
            var clock = new AlarmClock("Morning");
            var coffee = new CoffeeMachine();
            clock.OnAlarmRing += coffee.StartBrewing;

            ConsoleCapture.Run(() => { clock.Ring(SixThirty); clock.Ring(SixThirty.AddMinutes(10)); });

            Assert.Equal(2, coffee.CupsBrewed);
            Assert.Equal(SixThirty.AddMinutes(10), coffee.LastBrewedAt);
        }

        [Fact]
        public void AlarmDemo_Run_ShowsSubscribeAndUnsubscribe()
        {
            string output = ConsoleCapture.Run(AlarmDemo.Run);

            Assert.Contains("Subscribers: 2", output);
            Assert.Contains("Pradeep wakes up at 06:30", output);
            Assert.Contains("Subscribers after unsubscribe: 1", output);
            Assert.Contains("(cup #2)", output);
            Assert.DoesNotContain("Pradeep wakes up at 06:40", output);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void Main_RunsEveryTaskInOrder()
        {
            string output = ConsoleCapture.Run(() => DelegatesEventsLambdas.Program.Main(Array.Empty<string>()));

            int t8 = output.IndexOf("=== Task 2.8", StringComparison.Ordinal);
            int t9 = output.IndexOf("=== Task 2.9", StringComparison.Ordinal);
            int t10 = output.IndexOf("=== Task 2.10", StringComparison.Ordinal);

            Assert.True(t8 >= 0 && t8 < t9 && t9 < t10);
        }
    }
}
