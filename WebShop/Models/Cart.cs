using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models.Interfaces;

namespace WebShop.Models
{

    public class Cart : ICart
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public bool IsCheckedOut { get; set; } = false;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();



    }
}
