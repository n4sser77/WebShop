using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Managers;
using WebShop.Models.Interfaces;

namespace WebShop.Models
{
    public class WebShop(IWebSHopSettings settings, IProductManager productManager, IUserManager userManager, ICartManager cartManager, IOrderManager orderManager)
    {
        public int Id { get; set; }
        public IWebSHopSettings Settings { get; set; } = settings;
        public string ShopName { get; set; } = settings.ShopName;
        public IProductManager ProductManager { get; set; } = productManager;
        public IUserManager UserManager { get; set; } = userManager;
        public ICartManager CartManager { get; set; } = cartManager;

        public IOrderManager OrderManager { get; set; } = orderManager;
    }
}
