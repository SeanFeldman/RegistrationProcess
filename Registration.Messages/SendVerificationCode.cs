namespace Registration.Messages
{
    using NServiceBus;

    public class SendVerificationCode : ICommand
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }

    }
}