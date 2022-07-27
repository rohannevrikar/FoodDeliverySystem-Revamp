using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Orders_Handler_Service.Enums;

namespace Orders_Handler_Service
{
    public class MessageHandlerService : BackgroundService
    {
        private readonly ILogger<MessageHandlerService> _logger;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IConfiguration _configuration;
        private readonly Container container;

        private readonly ServiceBusReceiver _newOrdersReceiver;
        private readonly ServiceBusReceiver _acceptedOrdersReceiver;
        private readonly ServiceBusReceiver _outForDeliveryOrdersReceiver;
        private readonly ServiceBusReceiver _deliveredOrderReceiver;
        List<Task> tasks = new List<Task>();

        public MessageHandlerService(
            ILogger<MessageHandlerService> logger,
            IConfiguration configuration,
            CosmosClient cosmosClient,
            ServiceBusClient serviceBusClient)
        {
            _logger = logger;
            _configuration = configuration;
            container = cosmosClient.GetContainer(configuration["databaseName"], configuration["containerName"]);

            _serviceBusClient = serviceBusClient;
            _newOrdersReceiver = _serviceBusClient.CreateReceiver(_configuration["topic-orders-new"], _configuration["sub-orders-new"]);
            _acceptedOrdersReceiver = _serviceBusClient.CreateReceiver(_configuration["topic-orders-accepted"], _configuration["sub-orders-accepted"]);
            _outForDeliveryOrdersReceiver = _serviceBusClient.CreateReceiver(_configuration["topic-orders-out-for-delivery"], _configuration["sub-orders-out-for-delivery"]);
            _deliveredOrderReceiver = _serviceBusClient.CreateReceiver(_configuration["topic-orders-delivered"], _configuration["sub-orders-delivered"]);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Checking for orders...");

                CheckOrders();

                await Task.Delay(10000, stoppingToken); // checks for orders in topics every 10 seconds
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
        }

        private async void CheckOrders()
        {
            tasks.Add(NewOrderHandler());
            tasks.Add(AcceptedOrderHandler());
            tasks.Add(OutForDeliveryOrderHandler());
            tasks.Add(DeliveredOrderHandler());
            await Task.WhenAll(tasks);
            tasks.Clear();
        }

        private async Task NewOrderHandler()
        {
            foreach (var order in await _newOrdersReceiver.ReceiveMessagesAsync(maxMessages: 5))
            {
                Order newOrder = JsonConvert.DeserializeObject<Order>(order.Body.ToString());
                var item = await container.CreateItemAsync(newOrder);
                if (item != null)
                {
                    _logger.LogInformation("Order placed " + newOrder.Id);
                    _logger.LogInformation("RU: " + item.RequestCharge);
                    await _newOrdersReceiver.CompleteMessageAsync(order);
                }
            }
            await Task.CompletedTask;
        }

        private async Task AcceptedOrderHandler()
        {
            foreach (var order in await _acceptedOrdersReceiver.ReceiveMessagesAsync(maxMessages: 5))
            {
                if (await UpdateOrderStatus(order, OrderStatus.Accepted) != null)
                {
                    await _acceptedOrdersReceiver.CompleteMessageAsync(order);
                }
            }
            await Task.CompletedTask;
        }

        private async Task OutForDeliveryOrderHandler()
        {
            foreach (var order in await _outForDeliveryOrdersReceiver.ReceiveMessagesAsync(maxMessages: 5))
            {
                if (await UpdateOrderStatus(order, OrderStatus.OutForDelivery) != null)
                {
                    await _outForDeliveryOrdersReceiver.CompleteMessageAsync(order);
                }
            }
            await Task.CompletedTask;
        }

        private async Task DeliveredOrderHandler()
        {
            foreach (var order in await _deliveredOrderReceiver.ReceiveMessagesAsync(maxMessages: 5))
            {
                if (await UpdateOrderStatus(order, OrderStatus.Delivered) != null)
                {
                    await _deliveredOrderReceiver.CompleteMessageAsync(order);
                }
            }
            await Task.CompletedTask;
        }

        private async Task<ItemResponse<dynamic>> UpdateOrderStatus(ServiceBusReceivedMessage order, OrderStatus orderStatus)
        {
            Order orderToUpdate = JsonConvert.DeserializeObject<Order>(order.Body.ToString());

            List<PatchOperation> patchOperations =
                    new List<PatchOperation>() { PatchOperation.Replace("/orderStatus", orderStatus) };

            var item = await this.container.PatchItemAsync<dynamic>(orderToUpdate.Id, new PartitionKey(orderToUpdate.Id), patchOperations);

            _logger.LogInformation("Order updated " + orderToUpdate.Id + " to status " + orderStatus.ToString());
            _logger.LogInformation(" Patch item RU: " + item.RequestCharge);

            return item;
        }
    }
}
