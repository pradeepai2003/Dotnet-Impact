using ExtensionsMiniProjects.MiniProjects.Notifications;

namespace ExtensionsMiniProjects.Tests
{
    // Mini Q5 - Notification Engine
    public class NotificationEngineTests
    {
        [Fact]
        public void Send_FiresEventWithDetails()
        {
            NotificationService service = NotificationService.CreateDefault();
            NotificationSentEventArgs? received = null;
            object? receivedSender = null;
            service.OnNotificationSent += (sender, e) => { receivedSender = sender; received = e; };
            DateTime before = DateTime.Now;

            bool ok = false;
            ConsoleCapture.Run(() => ok = service.Send("Email", "asha@example.com", "Hi"));

            Assert.True(ok);
            Assert.Same(service, receivedSender);
            Assert.NotNull(received);
            Assert.Equal("Email", received!.Channel);
            Assert.Equal("asha@example.com", received.Recipient);
            Assert.Equal("Hi", received.Message);
            Assert.True(received.Success);
            Assert.InRange(received.SentAt, before, DateTime.Now);
        }

        [Fact]
        public void Send_FailedDelivery_StillFiresEvent_WithSuccessFalse()
        {
            NotificationService service = NotificationService.CreateDefault();
            var logger = new ConsoleNotificationLogger();
            logger.Subscribe(service);

            bool ok = true;
            ConsoleCapture.Run(() => ok = service.Send("Email", "not-an-email", "Hi"));

            Assert.False(ok);
            Assert.Equal("[LOG] Email to not-an-email: FAILED", Assert.Single(logger.Entries));
        }

        [Fact]
        public void Subscribers_LogToConsole_AndCount()
        {
            NotificationService service = NotificationService.CreateDefault();
            var logger = new ConsoleNotificationLogger();
            var stats = new NotificationStats();
            logger.Subscribe(service);
            stats.Subscribe(service);

            string output = ConsoleCapture.Run(() =>
            {
                service.Send("Email", "a@b.com", "one");
                service.Send("Sms", "9876543210", "two");
                service.Send("Push", "device-1", "three");
                service.Send("Sms", "123", "bad number");
            });

            Assert.Equal(4, logger.Entries.Count);
            Assert.Equal(3, stats.Sent);
            Assert.Equal(1, stats.Failed);
            Assert.Contains("[LOG] Email to a@b.com: SENT", output);
            Assert.Contains("[LOG] Push to device-1: SENT", output);
            Assert.Contains("[LOG] Sms to 123: FAILED", output);
        }

        [Fact]
        public void Unsubscribe_StopsLogging()
        {
            NotificationService service = NotificationService.CreateDefault();
            var logger = new ConsoleNotificationLogger();
            logger.Subscribe(service);
            logger.Unsubscribe(service);

            ConsoleCapture.Run(() => service.Send("Push", "device-1", "Hi"));

            Assert.Empty(logger.Entries);
        }

        [Fact]
        public void Send_WithNoSubscribers_DoesNotThrow()
        {
            NotificationService service = NotificationService.CreateDefault();

            bool ok = false;
            ConsoleCapture.Run(() => ok = service.Send("Push", "device-1", "Hi"));

            Assert.True(ok);
        }

        [Fact]
        public void Send_UnknownChannel_Throws_AndDoesNotFireEvent()
        {
            NotificationService service = NotificationService.CreateDefault();
            int fired = 0;
            service.OnNotificationSent += (_, _) => fired++;

            var ex = Assert.Throws<ArgumentException>(() => service.Send("Fax", "000", "Hi"));

            Assert.Contains("No sender registered for channel 'Fax'", ex.Message);
            Assert.Equal(0, fired);
        }

        [Fact]
        public void Channels_AreCaseInsensitive()
        {
            NotificationService service = NotificationService.CreateDefault();

            bool ok = false;
            ConsoleCapture.Run(() => ok = service.Send("EMAIL", "a@b.com", "Hi"));

            Assert.True(ok);
            Assert.Equal(3, service.Channels.Count);
        }

        [Fact]
        public void RegisterSender_CustomDelegate_IsUsed()
        {
            var service = new NotificationService();
            var delivered = new List<string>();
            service.RegisterSender("Slack", (to, msg) => { delivered.Add($"{to}:{msg}"); return true; });

            Assert.True(service.Send("slack", "#general", "deploy done"));
            Assert.Equal("#general:deploy done", Assert.Single(delivered));
        }

        [Fact]
        public void RegisterSender_SameChannelTwice_ReplacesSender()
        {
            var service = new NotificationService();
            service.RegisterSender("X", (_, _) => false);
            service.RegisterSender("x", (_, _) => true);

            Assert.True(service.Send("X", "r", "m"));
            Assert.Single(service.Channels);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void RegisterSender_BlankChannel_Throws(string channel)
        {
            Assert.Throws<ArgumentException>(() => new NotificationService().RegisterSender(channel, (_, _) => true));
        }

        [Fact]
        public void RegisterSender_NullSender_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new NotificationService().RegisterSender("Email", null!));
        }
    }

    // The three delegate-compatible senders.
    public class SenderTests
    {
        [Theory]
        [InlineData("a@b.com", "Hi", true)]
        [InlineData("no-at-sign", "Hi", false)]
        [InlineData("a@b.com", " ", false)]
        public void Email(string recipient, string message, bool expected)
        {
            bool ok = !expected;
            string output = ConsoleCapture.Run(() => ok = Senders.Email(recipient, message));

            Assert.Equal(expected, ok);
            Assert.Contains(expected ? "[Email] to" : "failed", output);
        }

        [Theory]
        [InlineData("9876543210", "Hi", true)]
        [InlineData("987654321", "Hi", false)]      // 9 digits
        [InlineData("98765abcde", "Hi", false)]     // letters
        [InlineData("9876543210", "", false)]       // empty message
        public void Sms_RecipientAndMessageRules(string recipient, string message, bool expected)
        {
            bool ok = !expected;
            ConsoleCapture.Run(() => ok = Senders.Sms(recipient, message));

            Assert.Equal(expected, ok);
        }

        [Theory]
        [InlineData(160, true)]
        [InlineData(161, false)]
        public void Sms_LengthBoundary(int length, bool expected)
        {
            bool ok = !expected;
            ConsoleCapture.Run(() => ok = Senders.Sms("9876543210", new string('x', length)));

            Assert.Equal(expected, ok);
        }

        [Theory]
        [InlineData("device-1", "Hi", true)]
        [InlineData("", "Hi", false)]
        [InlineData("device-1", "", false)]
        public void Push(string recipient, string message, bool expected)
        {
            bool ok = !expected;
            ConsoleCapture.Run(() => ok = Senders.Push(recipient, message));

            Assert.Equal(expected, ok);
        }

        [Fact]
        public void Senders_MatchTheDelegateSignature()
        {
            NotificationSender[] senders = { Senders.Email, Senders.Sms, Senders.Push };
            Assert.Equal(3, senders.Length);
        }
    }

    public class NotificationDemoTests
    {
        [Fact]
        public void Demo_Run_FiresAndLogs()
        {
            string output = ConsoleCapture.Run(NotificationEngineDemo.Run);

            Assert.Contains("[LOG] Email to asha@example.com: SENT", output);
            Assert.Contains("[LOG] Email to not-an-email: FAILED", output);
            Assert.Contains("Sent: 3, Failed: 1", output);
            Assert.Contains("Unknown channel: No sender registered for channel 'Fax'.", output);
        }
    }
}
