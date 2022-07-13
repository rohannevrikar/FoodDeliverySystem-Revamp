using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Orders_API_Service.Models;
using Orders_API_Service.Services.Contracts;
using static Orders_API_Service.Models.Enums;

namespace Orders_API_Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly Container container;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;

        public OrderService(
            ILogger<OrderService> logger,
            IConfiguration configuration,
            ServiceBusClient serviceBusClient,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            _configuration = configuration; 
            container = cosmosClient.GetContainer(configuration["databaseName"], configuration["containerName"]);
        }

        public void DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var item = await container.ReadItemAsync<Order>(id: orderId, partitionKey: new PartitionKey(orderId));
            Console.WriteLine("RU: " + item.RequestCharge);
            return item;
        }

        public async Task<IEnumerable<Order>> GetOrders(string customerId)
        {
            string queryString = "SELECT * FROM Orders o";
            if (!string.IsNullOrWhiteSpace(customerId))
            {
                queryString += " where o.customerId = '" + customerId + "'";
            }

            List<Order> orders = new List<Order>();
            var query = new QueryDefinition(query: queryString);

            using FeedIterator<Order> feed = container.GetItemQueryIterator<Order>(
                queryDefinition: query
            );
            
            while (feed.HasMoreResults)
            {
                FeedResponse<Order> response = await feed.ReadNextAsync();
                _logger.LogInformation("RU: " + response.RequestCharge);
                foreach (Order item in response)
                {
                    orders.Add(item);
                }
            }

            return orders.AsEnumerable<Order>();
        }

        public async Task UpsertOrder(Order order)
        {
            // Pre order processing logic goes here

            await SendMessage(order);
        }

        private async Task SendMessage(Order order)
        {
            string topicName = string.Empty;
            Console.WriteLine(order.OrderStatus);
            switch (order.OrderStatus)
            {
                case OrderStatus.New:
                    topicName = _configuration["topic-orders-new"];
                    break;

                case OrderStatus.Accepted:
                    topicName = _configuration["topic-orders-accepted"];
                    break;

                case OrderStatus.OutForDelivery:
                    topicName = _configuration["topic-orders-out-for-delivery"];
                    break;

                case OrderStatus.Delivered:
                    topicName = _configuration["topic-orders-delivered"];
                    break;

                default: throw new Exception("Invalid order status");

            }

            string serializedOrder= JsonConvert.SerializeObject(order);

            ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);
            ServiceBusMessage message = new ServiceBusMessage(serializedOrder);
            await sender.SendMessageAsync(message);
        }
    }
}
