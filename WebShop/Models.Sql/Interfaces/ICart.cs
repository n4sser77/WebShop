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
        public List<Product> Products { get; set; }
        public decimal Total { get; set; }

    }
}
