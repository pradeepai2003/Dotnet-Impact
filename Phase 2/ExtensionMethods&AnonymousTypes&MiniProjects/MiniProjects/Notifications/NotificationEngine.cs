// Mini Q5 - Notification Engine: delegate-based senders + an event that subscribers log.
namespace ExtensionsMiniProjects.MiniProjects.Notifications
{
    // Any method with this shape can be plugged in as a sender.
    public delegate bool NotificationSender(string recipient, string message);

    public class NotificationSentEventArgs : EventArgs
    {
        public string Channel { get; }
        public string Recipient { get; }
        public string Message { get; }
        public bool Success { get; }
        public DateTime SentAt { get; }

        public NotificationSentEventArgs(string channel, string recipient, string message, bool success, DateTime sentAt)
        {
            Channel = channel;
            Recipient = recipient;
            Message = message;
            Success = success;
            SentAt = sentAt;
        }
    }

    // Three senders matching the NotificationSender delegate.
    public static class Senders
    {
        public static bool Email(string recipient, string message)
        {
            bool ok = recipient.Contains('@') && !string.IsNullOrWhiteSpace(message);
            Console.WriteLine(ok
                ? $"  [Email] to {recipient}: {message}"
                : $"  [Email] to '{recipient}' failed: invalid address or empty message");
            return ok;
        }

        public static bool Sms(string recipient, string message)
        {
            bool ok = recipient.Length == 10 && recipient.All(char.IsDigit) && message.Length is > 0 and <= 160;
            Console.WriteLine(ok
                ? $"  [SMS] to {recipient}: {message}"
                : $"  [SMS] to '{recipient}' failed: need a 10-digit number and 1-160 characters");
            return ok;
        }

        public static bool Push(string recipient, string message)
        {
            bool ok = !string.IsNullOrWhiteSpace(recipient) && !string.IsNullOrWhiteSpace(message);
            Console.WriteLine(ok
                ? $"  [Push] to device {recipient}: {message}"
                : "  [Push] failed: device id and message are required");
            return ok;
        }
    }

    // PUBLISHER
    public class NotificationService
    {
        private readonly Dictionary<string, NotificationSender> _senders = new(StringComparer.OrdinalIgnoreCase);

        public event EventHandler<NotificationSentEventArgs>? OnNotificationSent;

        public IReadOnlyCollection<string> Channels => _senders.Keys;

        public void RegisterSender(string channel, NotificationSender sender)
        {
            if (string.IsNullOrWhiteSpace(channel))
                throw new ArgumentException("Channel is required.", nameof(channel));
            ArgumentNullException.ThrowIfNull(sender);
            _senders[channel] = sender;
        }

        public bool Send(string channel, string recipient, string message)
        {
            if (!_senders.TryGetValue(channel, out NotificationSender? sender))
                throw new ArgumentException($"No sender registered for channel '{channel}'.", nameof(channel));

            bool success = sender(recipient, message);
            // Fire the event for every attempt - successful or failed - so subscribers can log both.
            OnNotificationSent?.Invoke(this, new NotificationSentEventArgs(channel, recipient, message, success, DateTime.Now));
            return success;
        }

        public static NotificationService CreateDefault()
        {
            var service = new NotificationService();
            service.RegisterSender("Email", Senders.Email);
            service.RegisterSender("Sms", Senders.Sms);
            service.RegisterSender("Push", Senders.Push);
            return service;
        }
    }

    // SUBSCRIBER 1: writes a log line for each notification.
    public class ConsoleNotificationLogger
    {
        public List<string> Entries { get; } = new();

        public void Subscribe(NotificationService service) => service.OnNotificationSent += HandleNotificationSent;

        public void Unsubscribe(NotificationService service) => service.OnNotificationSent -= HandleNotificationSent;

        private void HandleNotificationSent(object? sender, NotificationSentEventArgs e)
        {
            string entry = $"[LOG] {e.Channel} to {e.Recipient}: {(e.Success ? "SENT" : "FAILED")}";
            Entries.Add(entry);
            Console.WriteLine(entry);
        }
    }

    // SUBSCRIBER 2: keeps counts.
    public class NotificationStats
    {
        public int Sent { get; private set; }
        public int Failed { get; private set; }

        public void Subscribe(NotificationService service) => service.OnNotificationSent += (_, e) =>
        {
            if (e.Success) Sent++;
            else Failed++;
        };
    }

    public static class NotificationEngineDemo
    {
        public static void Run()
        {
            NotificationService service = NotificationService.CreateDefault();
            var logger = new ConsoleNotificationLogger();
            var stats = new NotificationStats();
            logger.Subscribe(service);
            stats.Subscribe(service);

            service.Send("Email", "asha@example.com", "Your payslip is ready");
            service.Send("sms", "9876543210", "OTP 482913");
            service.Send("Push", "device-42", "New message from Ravi");
            service.Send("Email", "not-an-email", "This will fail");

            Console.WriteLine($"Sent: {stats.Sent}, Failed: {stats.Failed}");

            try
            {
                service.Send("Fax", "0000", "Hello");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Unknown channel: {ex.Message}");
            }
        }
    }
}
