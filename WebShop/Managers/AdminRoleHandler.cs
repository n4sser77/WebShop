using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models;
using WebShop.Models.Interfaces;
using WebShop.Models.Sql.Interfaces;
using WindowGUI;
using ZstdSharp.Unsafe;

namespace WebShop.Managers
{
    internal class AdminRoleHandler : IUserRoleHandler
    {

        private readonly IProductManager _productManager;
        private readonly ICartManager _cartManager;


        private readonly IOrderManager _orderManager;
        private readonly IUserManager _userManager = new UserManager();

        public AdminRoleHandler(IProductManager productManager, ICartManager cartManager, IUserManager userManager, IOrderManager orderManager)
        {
            _productManager = productManager;
            _cartManager = cartManager;

            IOrderManager OrderManager = orderManager;
            IUserManager UserManager = userManager;

        }

        public async Task<bool> HandleUserSessionAsync(User user)
        {
            throw new NotImplementedException();
        }




    }
}

