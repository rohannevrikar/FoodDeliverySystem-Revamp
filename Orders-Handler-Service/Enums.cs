using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders_Handler_Service
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
