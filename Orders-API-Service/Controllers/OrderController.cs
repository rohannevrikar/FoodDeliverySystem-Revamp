using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Orders_API_Service.Models;
using Orders_API_Service.Services.Contracts;

namespace Orders_API_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            ILogger<OrderController> logger,
            IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get([FromQuery] string? customerId)
        {
            return await _orderService.GetOrders(customerId);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<Order> GetById([FromRoute]string Id)
        {
            return await _orderService.GetOrderById(Id);
        }

        [HttpPost]
        public async Task<ActionResult> UpsertOrder(Order order)
        {            
            await _orderService.UpsertOrder(order);  
            return Created(String.Empty, order);
        }
    }
}
