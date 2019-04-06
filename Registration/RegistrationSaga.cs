namespace Registration
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;
    using NServiceBus.Logging;

    public class RegistrationSaga : Saga<RegistrationData>,
        IAmStartedByMessages<RegisterNewUser>,
        IHandleMessages<ConfirmEmail>,
        IHandleTimeouts<ReminderAfter24Hours>,
        IHandleTimeouts<ReminderAfter48Hours>
    {
        static ILog log = LogManager.GetLogger<RegistrationSaga>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RegistrationData> mapper)
        {
            mapper.ConfigureMapping<RegisterNewUser>(message => message.Email)
                .ToSaga(sagaData => sagaData.Email);

            mapper.ConfigureMapping<ConfirmEmail>(message => message.Email)
                .ToSaga(sagaData => sagaData.Email);
        }

        public async Task Handle(RegisterNewUser message, IMessageHandlerContext context)
        {
            log.Info($"Received registration intent for user with email: {message.Email}. Yes, no GDPR here ;)");

            Data.VerificationCode = Guid.NewGuid().ToString("D").Substring(0, 4);

            await context.SendLocal(new SendVerificationCode
            {
                Email = message.Email,
                VerificationCode = Data.VerificationCode
            });

            await RequestTimeout<ReminderAfter24Hours>(context, TimeSpan.FromSeconds(2.4));

            await RequestTimeout<ReminderAfter48Hours>(context, TimeSpan.FromSeconds(4.8));
        }

        public async Task Handle(ConfirmEmail message, IMessageHandlerContext context)
        {
            log.Info($"Received confirmation intent for user with email: {message.Email}.");

            Data.Confirmed = message.VerificationCode == Data.VerificationCode;

            if (!Data.Confirmed)
            {
                log.Info($"Email {message.Email} was attempted for verification with a wrong confirmation code.");
                return;
            }

            // send an internal command to persist email and password

            await context.Publish(new RegistrationCompleted { EmailAddress = message.Email });

            MarkAsComplete();

            log.Info($"Email {message.Email} verified. We've got a new user");
        }

        public Task Timeout(ReminderAfter24Hours state, IMessageHandlerContext context)
        {
            log.Info("24 hours since registration have elapsed and no confirmation was received. Going to remind.");

            return Task.CompletedTask;
        }

        public Task Timeout(ReminderAfter48Hours state, IMessageHandlerContext context)
        {
            log.Info("48 hours since registration have elapsed and no confirmation. Au revoir!");

            MarkAsComplete();

            return Task.CompletedTask;
        }
    }
}