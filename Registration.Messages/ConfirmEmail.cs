namespace Registration.Messages
{
    using NServiceBus;

    public class ConfirmEmail : ICommand
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}