using System;

namespace ClientApp
{
    using System.Threading.Tasks;
    using NServiceBus;
    using Registration.Messages;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Client UI";

            var endpointConfiguration = new EndpointConfiguration("client");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableInstallers();

            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(connectionString);

            transport.Routing().RouteToEndpoint(typeof(RegisterNewUser), "registration");
            transport.Routing().RouteToEndpoint(typeof(ConfirmEmail), "registration");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press 'r' to register");
            Console.WriteLine("Press 'c' to confirm");
            Console.WriteLine("Press 'Esc' to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

                if (key.Key == ConsoleKey.R)
                {
                    await endpointInstance.Send(new RegisterNewUser
                    {
                        Email = "feldman.sean@gmail.com",
                        Password = "1234"
                    });

                    Console.WriteLine("Registration sent");
                }
                else if (key.Key == ConsoleKey.C)
                {
                    var verificationCode = Console.ReadLine();

                    await endpointInstance.Send(new ConfirmEmail
                    {
                        Email = "feldman.sean@gmail.com",
                        VerificationCode = verificationCode
                    });

                    Console.WriteLine("Confirmation sent");

                }
            }

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
