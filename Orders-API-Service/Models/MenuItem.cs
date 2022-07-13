using Newtonsoft.Json;

namespace Orders_API_Service.Models
{
    public class MenuItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "item")]
        public string Item { get; set; }

        [JsonProperty(PropertyName = "price")]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "dishType")]
        public DishType DishType { get; set; }
    }
    public enum DishType
    {
        Veg = 0,
        NonVeg = 1,
        ContainsEgg = 2
    }
}
