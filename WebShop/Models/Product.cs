using System.ComponentModel.DataAnnotations;
using WebShop.Models.Interfaces;

namespace WebShop.Models
{
    public class Product : IProduct
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]

        public bool IsDeleted { get; set; }



        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
