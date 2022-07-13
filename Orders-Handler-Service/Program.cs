using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders_Handler_Service;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton((s) =>
        {
            return new CosmosClient(hostContext.Configuration["CosmosEndpoint"], new DefaultAzureCredential());
        }); // Using managed identities

        services.AddSingleton((s) =>
        {
            return new ServiceBusClient(hostContext.Configuration["ServiceBusEndpoint"], new DefaultAzureCredential());
        }); // Using managed identities

        services.AddHostedService<MessageHandlerService>();

    })
    .RunConsoleAsync();

