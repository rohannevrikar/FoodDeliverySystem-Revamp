using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders_Handler_Service
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "restaurantId")]
        public string RestaurantId { get; set; }

        [JsonProperty(PropertyName = "restaurantName")]
        public string RestaurantName { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string CustomerId { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string CustomerFirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string CustomerLastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "contactNumber")]
        public string ContactNumber { get; set; }

        [JsonProperty(PropertyName = "orderStatus")]
        public Enums.OrderStatus OrderStatus { get; set; }

        [JsonProperty(PropertyName = "orderItems")]
        public List<MenuItem> OrderItems { get; set; }
    }
}
