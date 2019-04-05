namespace ClientApp
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using Registration.Messages;

    public class RegistrationCompletedHandler : IHandleMessages<RegistrationCompleted>
    {
        static ILog log = LogManager.GetLogger<RegistrationCompletedHandler>();

        public Task Handle(RegistrationCompleted message, IMessageHandlerContext context)
        {
            log.Info($"Registration with email {message.EmailAddress} is completed.");

            return Task.CompletedTask;
        }
    }
}