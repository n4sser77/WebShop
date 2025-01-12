using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class WebShopSettings : IWebSHopSettings
    {
        public required string ShopName { get; set; }

    }
}
