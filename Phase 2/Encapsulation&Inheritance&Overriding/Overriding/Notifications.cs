// Task 2.3 - virtual / override / sealed override.
namespace EncapsulationInheritanceOverriding.Overriding
{
    public class Notification
    {
        public string Recipient { get; }

        public Notification(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Recipient is required.", nameof(recipient));
            Recipient = recipient;
        }

        public virtual string Send(string message) => $"[Generic] To {Recipient}: {message}";
    }

    public class EmailNotification : Notification
    {
        public string Subject { get; init; } = "(no subject)";

        public EmailNotification(string recipient) : base(recipient) { }

        // 'sealed override': overrides Notification.Send, but NO class below this one may override it again.
        public sealed override string Send(string message) =>
            $"[Email] To {Recipient}, Subject \"{Subject}\": {message}";
    }

    public class SmsNotification : Notification
    {
        public const int MaxLength = 160;

        public SmsNotification(string phoneNumber) : base(phoneNumber) { }

        public override string Send(string message)
        {
            string body = message.Length > MaxLength ? message[..(MaxLength - 3)] + "..." : message;
            return $"[SMS] To {Recipient}: {body}";
        }
    }

    public class PushNotification : Notification
    {
        public PushNotification(string deviceId) : base(deviceId) { }

        public override string Send(string message) => $"[Push] To device {Recipient}: {message}";
    }

    public class PriorityEmailNotification : EmailNotification
    {
        public PriorityEmailNotification(string recipient) : base(recipient) { }

        // public override string Send(string message) => "[Priority] " + message;
        //   ERROR CS0239: 'PriorityEmailNotification.Send(string)': cannot override inherited member
        //                 'EmailNotification.Send(string)' because it is sealed
        //   WHY: EmailNotification locked the behaviour with 'sealed override'.

        // The sealed method can still be CALLED - it just can't be replaced.
        public string SendUrgent(string message) => Send("URGENT: " + message);
    }

    public static class NotificationDemo
    {
        public static void Run()
        {
            List<Notification> notifications = new()
            {
                new Notification("admin"),
                new EmailNotification("asha@example.com") { Subject = "Welcome" },
                new SmsNotification("9876543210"),
                new PushNotification("device-42"),
            };

            foreach (Notification notification in notifications)
            {
                // The runtime type decides which Send() runs (virtual dispatch).
                Console.WriteLine(notification.Send("Your order has shipped"));
            }

            var priority = new PriorityEmailNotification("ops@example.com") { Subject = "Server down" };
            Console.WriteLine(priority.SendUrgent("Database is not responding"));
        }
    }
}
