namespace Registration.Messages
{
    using NServiceBus;

    public class RegisterNewUser : ICommand
    {
        public string Email { get; set; }

        // TODO: use encrypted property https://docs.particular.net/nservicebus/security/property-encryption
        public string Password { get; set; }
    }
}