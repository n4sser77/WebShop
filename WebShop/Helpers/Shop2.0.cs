//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WebShop.Managers;
//using WebShop.Models.Interfaces;
//using WebShop.Models.Sql.Interfaces;
//using WebShop.Models;

//namespace WebShop.Helpers
//{
//    public class Shop2
//    {
//        private Models.WebShop _WebShop { get; set; }
//        private bool _IsLoggedIn { get; set; } = false;
//        private User? _currentUser { get; set; }
//        private bool isInDev = true;
//        private IUserRoleHandler _currentRoleHandler { get; set; }
//        public Shop2(string shopName)
//        {
//            ICartManager CartManager = new CartManager();
//            IProductManager ProductManager = new ProductManager();
//            IOrderManager OrderManager = new OrderManager();
//            IUserManager UserManager = new UserManager();

//            WebShopSettings Settings = new WebShopSettings
//            {
//                ShopName = shopName
//            };
//            _WebShop = new Models.WebShop(Settings, ProductManager, UserManager, CartManager, OrderManager);
//        }


//        public async Task Run()
//        {
//            // ... existing dev login code ...

//            while (true)
//            {
//                if (_IsLoggedIn && _currentUser != null)
//                {
//                    // Get result from role handler
//                    bool continueSession = await _currentRoleHandler.HandleUserSessionAsync(_currentUser);

//                    if (!continueSession)
//                    {
//                        _IsLoggedIn = false;
//                        _currentUser = null;
//                    }
//                }
//                else
//                {
//                    WelcomeUser();
//                    await UserInput();
//                }
//            }
//        }
//    }
//}
