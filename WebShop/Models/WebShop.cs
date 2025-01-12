using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models.Interfaces;
using WebShop.Models.Managers;

namespace WebShop.Models
{
    public class WebShop(IWebSHopSettings settings, ProductManager productManager, UserManager userManager, CartManager cartManager, OrderManager orderManager)
    {
        public int Id { get; set; }
        public IWebSHopSettings Settings { get; set; } = settings;
        public string ShopName { get; set; } = settings.ShopName;
        public ProductManager ProductManager { get; set; } = productManager;
        public UserManager UserManager { get; set; } = userManager;
        public CartManager CartManager { get; set; } = cartManager;

        public OrderManager OrderManager { get; set; } = orderManager;
    }
}
