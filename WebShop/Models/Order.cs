using System.ComponentModel.DataAnnotations;
using WebShop.Models.Interfaces;

namespace WebShop.Models
{
    public class Order : ICart
    {
        public int Id { get; set; }
        [Required]

        public double Total { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public virtual int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
