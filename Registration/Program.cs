using System;

namespace Registration
{
    using System.Threading.Tasks;
    using Messages;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Registration Service";

            var endpointConfiguration = new EndpointConfiguration("registration");
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString"));

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Sagas>();
            persistence.ConnectionString(Environment.GetEnvironmentVariable("AzureStoragePersistence_ConnectionString"));
            persistence.AssumeSecondaryIndicesExist();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
