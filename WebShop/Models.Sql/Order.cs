using System.ComponentModel.DataAnnotations;
using WebShop.Models.Interfaces;

namespace WebShop.Models
{
    public class Order : ICart
    {
        public int Id { get; set; }
        [Required]

        public decimal Total { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public virtual int CustomerId { get; set; }
        public required virtual User Customer { get; set; }


        public virtual List<Product> Products { get; set; } = new List<Product>();

    }
}
