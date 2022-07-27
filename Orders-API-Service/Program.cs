using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Orders_API_Service.Services;
using Orders_API_Service.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderService, OrderService>();

//builder.Services.AddSingleton((s) =>
//{
//    return new CosmosClient(hostContext.Configuration["CosmosEndpoint"], new DefaultAzureCredential());
//}); // Using managed identities


//builder.Services.AddSingleton((s) =>
//{
//    return new ServiceBusClient(hostContext.Configuration["ServiceBusEndpoint"], new DefaultAzureCredential());

//}); // Using managed identities

builder.Services.AddSingleton((s) =>
{
    return new CosmosClient(builder.Configuration.GetConnectionString("CosmosDBConnectionString"));
}); // Using connection string for now



builder.Services.AddSingleton((s) =>
{
    return new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBusConnectionString"));

}); // Using connection string for now

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
