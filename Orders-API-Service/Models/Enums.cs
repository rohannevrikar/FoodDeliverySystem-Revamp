namespace Orders_API_Service.Models
{
    public class Enums
    {
        public enum ItemTypes
        {
            Order,
            Customer,
            Restaurant
        }
        public enum OrderStatus
        {
            Unassigned,
            New,
            Accepted,
            OutForDelivery,
            Delivered,
            DeliveryFailed,
            Canceled
        }
    }
}
