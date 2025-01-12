using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models.Interfaces
{
    public interface ICart
    {
        public int Id { get; set; }
        public ICollection<Product> Products { get; set; }
        public double Total { get; set; }

    }
}
