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

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            transport.Routing().RouteToEndpoint(typeof(RegisterNewUser), "registration");
            transport.Routing().RouteToEndpoint(typeof(ConfirmEmail), "registration");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
