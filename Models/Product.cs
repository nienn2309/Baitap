using System.ComponentModel.DataAnnotations;

namespace Baitap.Models
{
    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string imageUrl { get; set; }
        public string description { get; set; }
    }

    public class ProductResponse
    {
        public List<Product> items { get; set; }
        public int totalItems { get; set; }
    }

    public class DeleteResponse
    {
        public string message { get; set; }
    }
}
