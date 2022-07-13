using Orders_API_Service.Models;

namespace Orders_API_Service.Services.Contracts
{
    public interface IOrderService
    {
        public Task<IEnumerable<Order>> GetOrders(string customerId);

        public Task<Order> GetOrderById(string id);

        public Task UpsertOrder(Order order);

        public void DeleteOrder(int id);
    }
}
