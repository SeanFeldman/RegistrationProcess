namespace Registration
{
    using NServiceBus;

    public class RegistrationData : ContainSagaData
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public bool Confirmed { get; set; }
    }
}