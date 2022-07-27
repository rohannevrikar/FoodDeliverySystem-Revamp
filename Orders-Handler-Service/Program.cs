using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orders_Handler_Service;

//IConfigurationBuilder builder = new ConfigurationBuilder().AddUserSecrets<Program>();
//IConfigurationRoot root = builder.Build();

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        //services.AddSingleton((s) =>
        //{
        //    return new CosmosClient(hostContext.Configuration["CosmosEndpoint"], new DefaultAzureCredential());
        //}); // Using managed identities


        //services.AddSingleton((s) =>
        //{
        //    return new ServiceBusClient(hostContext.Configuration["ServiceBusEndpoint"], new DefaultAzureCredential());

        //}); // Using managed identities

        services.AddSingleton((s) =>
        {
            return new CosmosClient(hostContext.Configuration.GetConnectionString("CosmosDBConnectionString"));
        }); // Using connection string for now


        services.AddSingleton((s) =>
        {
            return new ServiceBusClient(hostContext.Configuration.GetConnectionString("ServiceBusConnectionString"));

        }); // Using connection string for now

        services.AddHostedService<MessageHandlerService>();

    })
    .RunConsoleAsync();

