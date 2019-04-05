namespace Registration
{
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;
    using NServiceBus.Logging;

    public class SendVerificationCodeHandler : IHandleMessages<SendVerificationCode>
    {
        static ILog log = LogManager.GetLogger<SendVerificationCodeHandler>();

        public Task Handle(SendVerificationCode message, IMessageHandlerContext context)
        {
            log.Info($"Sending verification code {message.VerificationCode} to {message.Email}");

            // email user the code

            return Task.CompletedTask;
        }
    }
}