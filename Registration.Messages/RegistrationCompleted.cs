namespace Registration.Messages
{
    using NServiceBus;

    public class RegistrationCompleted : IEvent
    {
        public string EmailAddress { get; set; }
    }
}