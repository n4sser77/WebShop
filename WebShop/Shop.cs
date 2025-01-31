using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebShop.Data;
using WebShop;


using WebShop.Managers;
using WebShop.Models;
using WebShop.Models.Interfaces;
using WebShop.Models.Sql.Interfaces;
using WindowGUI;




namespace WebShop
{
    public class Shop
    {

        private Models.WebShop _WebShop { get; set; }
        private bool _IsLoggedIn { get; set; } = false;
        private User? _currentUser { get; set; }
        private bool isInDev = false;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private IUserRoleHandler _currentRoleHandler { get; set; }
        public Shop(string shopName)
        {
            ICartManager CartManager = new CartManager();
            IProductManager ProductManager = new ProductManager();
            IOrderManager OrderManager = new OrderManager();
            IUserManager UserManager = new UserManager();

            WebShopSettings Settings = new WebShopSettings
            {
                ShopName = shopName
            };
            _WebShop = new Models.WebShop(Settings, ProductManager, UserManager, CartManager, OrderManager);
        }



        public async Task Run()
        {
            Console.Clear();
            if (isInDev)
            {
                using var db = new AppDbContext();

                var admin = db.Users.FirstOrDefault(u => u.FirstName == "Naser");

                if (admin != null)
                {
                    _currentUser = admin;
                    _IsLoggedIn = true;
                }

            }

            while (true)
            {
                if (_IsLoggedIn && _currentUser.Role == "Admin")
                {
                    Console.Clear();



                    var window1 = new Window(_currentUser.Role, 0, 0, new List<string> { "Welcome " + _currentUser.FirstName + "!", "[X] to sign out" });
                    var toolbarWindow = new Window("Admin tools", 20, 0, new List<string> { "[D] to add a category", "[L] to list categories", "[B] to view best sellers ", "[W] to sales by city ", "[Y] to view popular categories " });
                    var toolbar1Window = new Window("Product tools", 72, 0, new List<string> { "[P] to add a product", "[A] to list products", "[H] to list all featured products", "[S] to search products", "[T] to view a products details", "[C] to view customers " });



                    cts.Cancel();
                    var k = Console.ReadKey(true);
                    switch (k.Key)
                    {
                        case ConsoleKey.X:
                            Console.Clear();
                            _IsLoggedIn = false;
                            _currentUser = null;
                            continue;
                        case ConsoleKey.Y:
                            var popularCategories = await _WebShop.ProductManager.GetPopularCategories();

                            foreach (var c in popularCategories)
                            {

                                UiHelper.ChangeConsoleColors(ConsoleColor.Red, ConsoleColor.Gray);
                                Console.WriteLine($"{c.Id,-6}{c.Name,-25}");
                                Console.ResetColor();
                                await DisplayProducts(c.Products.ToList(), isAdmin: true, skipUserInput: true);
                            }
                            await GetProductIdFromUser();
                            break;
                        case ConsoleKey.B:
                            var topProducts = await _WebShop.ProductManager.GetTopProductsAsync();
                            int topProductId = await DisplayProducts(topProducts, isAdmin: true);
                            await DisplayProductDetailsAdmin(topProductId);
                            continue;
                        case ConsoleKey.W:
                            await DisplayPopularProductsInCity();
                            break;
                        case ConsoleKey.C:
                            await ViewCustomers();
                            continue;
                        case ConsoleKey.T:
                            Console.Write("Enter product ID: ");
                            var idString = Console.ReadLine();
                            if (int.TryParse(idString, out int result))
                            {
                                await DisplayProductDetailsAdmin(result);
                            }
                            continue; ;
                        case ConsoleKey.S:
                            Console.Write("Enter product name: ");
                            await LiveSearchWithCancellationAsync();
                            Console.Write("Enter product ID: ");
                            var idStringFromSearch = Console.ReadLine();
                            if (int.TryParse(idStringFromSearch, out int resultFromSearch))
                            {
                                await DisplayProductDetailsAdmin(resultFromSearch);
                            }
                            break;
                        case ConsoleKey.H:
                            var feauteredProducts = await _WebShop.ProductManager.GetFeaturedProducts();
                            foreach (var fP in feauteredProducts)
                            {
                                Console.WriteLine($"{fP.Id,-7}{fP.Name,-30}");
                            }
                            break;
                        case ConsoleKey.A:
                            var productId = await DisplayProducts();
                            await DisplayProductDetailsAdmin(productId);
                            break;
                        case ConsoleKey.P:
                            await AddProduct();
                            break;
                        case ConsoleKey.D:
                            await AddCategory();
                            break;
                        case ConsoleKey.L:
                            await DisplayCategories();
                            break;
                        default:

                            break;
                    }

                    Console.WriteLine("Press enter again to continue");
                    Console.ReadKey();

                }
                else if (_IsLoggedIn && _currentUser.Role.ToLower() == "customer")
                {
                    Console.Clear();
                    var welcomeText = "Welcome " + _currentUser.FirstName + "!";
                    await DisplayLogo();


                    var window1 = new Window(_currentUser.Role, 0, 0, new List<string> { welcomeText });

                    await DisplayFeatured();
                    await DisplayCart();

                    var toolbarWindow = new Window("Customer tools", 84, 0, ["[S] to search for a product", "[L] to list all products", "[C] to list all categories", "[A] to add a product to cart", "[O] to view cart", "[D] to remove from cart", "[H] to view order history"], ConsoleColor.White, ConsoleColor.Black);

                    Console.WriteLine("\nPress X to sign out");
                    var k = Console.ReadKey(true);
                    switch (k.Key)
                    {
                        case ConsoleKey.X:
                            _currentUser = null;
                            _IsLoggedIn = false;
                            Console.Clear();
                            continue;
                        case ConsoleKey.H:
                            await ViewOrderHistory();
                            break;
                        case ConsoleKey.L:
                            var productDetailsId = await DisplayProducts();
                            await DisplayProductDetails(productDetailsId);
                            break;
                        case ConsoleKey.S:
                            Console.Write("Enter product name: ");
                            await LiveSearchWithCancellationAsync();
                            Console.Write("Enter product ID: ");
                            var idStringFromSearch = Console.ReadLine();
                            if (int.TryParse(idStringFromSearch, out int resultFromSearch))
                            {
                                await DisplayProductDetails(resultFromSearch);
                            }
                            break;
                        case ConsoleKey.D:
                            Console.Write("Enter product ID: ");
                            var idToRemove = Console.ReadLine();
                            if (string.IsNullOrEmpty(idToRemove)) break;
                            await _WebShop.CartManager.RemoveFromCart(_currentUser, idToRemove);
                            break;
                        case ConsoleKey.O:
                            await RenderCartMode();

                            break;
                        case ConsoleKey.A:
                            Console.Write("Enter product ID to enter to cart: ");
                            var id = Console.ReadLine();
                            if (string.IsNullOrEmpty(id)) break;

                            if (!int.TryParse(id, out int productId)) return;

                            var product = await _WebShop.ProductManager.GetProduct(productId);
                            if (product == null) { Console.WriteLine("Product not found"); return; };

                            await _WebShop.CartManager.AddToUserCart(_currentUser, product);
                            Console.WriteLine(product.Name + " added to cart");

                            continue;
                        case ConsoleKey.T:
                            Console.Write("Enter product ID");
                            var idString = Console.ReadLine();
                            if (int.TryParse(idString, out int result))
                            {
                                await DisplayProductDetails(result);
                            }
                            break;
                        default:
                            Console.WriteLine("Press enter again to continue");
                            break;


                    }

                    Console.ReadKey();
                }
                else
                {
                    WelcomeUser();

                    await UserInput();
                    continue;
                }
            }


        }


        private async Task DisplayPopularProductsInCity()
        {
            var popularProductsByCity = await _WebShop.ProductManager.GetPopularProductInCity();

            // Group products by country
            var groupedByCountry = popularProductsByCity
                .GroupBy(p => p.Country)
                .OrderBy(g => g.Key); // Optional: Sort countries alphabetically

            if (!groupedByCountry.Any())
            {
                Console.WriteLine("No products here");
                return;
            }

            // Display the products grouped by country
            foreach (var countryGroup in groupedByCountry)
            {
                UiHelper.ChangeConsoleColors(foreground: ConsoleColor.Black, background: ConsoleColor.White);
                Console.WriteLine();
                Console.WriteLine($"Country: {countryGroup.Key}");
                Console.WriteLine($"{"Id",-6}{"Name",-30}{"Copies sold",-10}");

                Console.ResetColor();
                foreach (var product in countryGroup)
                {
                    Console.WriteLine($"{product.Id,-6}{product.Name,-30}{$"{product.SoldCount}",-10}");
                }
            }
        }



        private async Task ViewOrderHistory()
        {
            Console.Clear();
            var orders = await _WebShop.OrderManager.OrdersToListAsync(_currentUser.Id);

            // Print headers for the orders
            Console.WriteLine($"{"Order ID",-10}{"Order Date",-30}{"Payment",-15}");
            Console.WriteLine(new string('-', 70));

            foreach (var o in orders)
            {
                Console.WriteLine($"{o.Id,-10}{o.OrderDate,-30}{o.PaymentMethod,-15}");


                // Print headers for the products
                Console.WriteLine(new string('-', 70));
                Console.WriteLine($"\t{"Product ID",-10}{"Product Name",-30}{"Price",-15}");
                Console.WriteLine("\t" + new string('-', 62));

                foreach (var p in o.Products)
                {
                    Console.WriteLine($"\t{p.Id,-10}{p.Name,-30}{p.Price,-15} SEK");
                }

                Console.WriteLine("\nTotal: " + o.Total);
                Console.WriteLine(new string('=', 70)); // Separator for each order
            }
        }

        private async Task RenderCartMode()
        {
            Console.Clear();
            UiHelper.ChangeConsoleColors(ConsoleColor.Black, ConsoleColor.White);
            Console.WriteLine("[C] to Checkout");
            Console.ResetColor();
            Console.WriteLine("--Cart------------------------------------------------------------------------------------------------------------------");
            var cart = await _WebShop.CartManager.GetUserCart(_currentUser);
            if (cart == null) return;

            Console.WriteLine($"{"Id",-6}{"Name",-30}{"Categories",-25}{"Price",-7}");
            foreach (var item in cart.Products)
            {
                Console.WriteLine($"{$"[{item.Id}]",-6}{item.Name,-35}{string.Join(',', item.Categories.Select(c => c.Name)),-35}{item.Price,-7} SEK");
            }

            var total = cart.Products.Sum(p => p.Price);
            Console.WriteLine($"\n{"Taxes",66}");
            var taxes = (float)total * 0.04;
            Console.WriteLine($"{taxes,67} SEK");
            Console.WriteLine($"\n{"Total",66}");
            Console.WriteLine($"{total,67} SEK");
            var k = Console.ReadKey(true);

            if (k.Key == ConsoleKey.C)
            {
                // Checkout logic, refactor and extract as a new method later
                var cartFromDb = await _WebShop.CartManager.GetUserCart(_currentUser);
                var checkoutCart = cartFromDb;
                if (checkoutCart == null) return;
                if (await _WebShop.CartManager.CheckoutCart(checkoutCart.Id)) // mark cart as checkedout to create an order and calc total
                {


                    Console.Write("Enter postal code: ");
                    var postalCode = Console.ReadLine();
                    if (string.IsNullOrEmpty(postalCode)) return;

                    Console.Write("Enter City: ");
                    var city = Console.ReadLine();
                    if (string.IsNullOrEmpty(city)) return;

                    Console.Write("Enter Country: ");
                    var country = Console.ReadLine();
                    if (string.IsNullOrEmpty(country)) return;

                    var phoneNumber = GetPhoneNumber();
                    var customer = _currentUser.FillCustomerDetails(postalCode, city, country, phoneNumber);

                    await _WebShop.UserManager.UpdateUser(customer.Id, null, null, null, phoneNumber, null, postalCode, country, city);
                    Console.WriteLine("Please choose payment method");
                    Console.Write("  Invoice Or Paypal: ");

                    var chosenMethod = Console.ReadLine();
                    if (string.IsNullOrEmpty(chosenMethod)) return;

                    if (customer == null) return;

                    var order = await _WebShop.OrderManager.CreateOrder(customer, chosenMethod);

                    await _WebShop.OrderManager.ProcessOrder(order.Id);

                    await _WebShop.CartManager.ClearCart(checkoutCart.Id);
                }

            }



        }

        private async Task DisplayCart()
        {
            var cart = await _WebShop.CartManager.GetUserCart(_currentUser);
            if (cart == null) return;
            var stringItems = new List<string>() { $"{"Id",-3}  {"Name",-25} {"Price",-7} " };

            foreach (var p in cart.Products)
            {
                stringItems.Add($"{$"[{p.Id}]",-4} {p.Name,-25} {p.Price,-7} SEK ");
                // stringItems.Add($"{p.Id,-4} {p.Name,-25} {p.Price,-5} SEK ");
            }
            // stringItems.Add("Total: " + cart.Sum(p => p.Price) + " SEK");
            stringItems.Add($"{"Total",36}");
            var total = cart.Products.Sum(p => p.Price);
            stringItems.Add($"{total,38} SEK");


            if (stringItems.Count == 0)
            {
                var emptyCart = new Window("Cart", 0, 4, new List<string> { "Cart is empty" });
                return;
            }
            var cartWindow = new Window("Cart", 0, 4, stringItems);


        }

        private async Task DisplayFeatured()
        {
            var featuredProducts = await _WebShop.ProductManager.GetFeaturedProducts();

            if (featuredProducts.Count == 0)
            {
                Console.WriteLine("No featured products");
            }
            int i = 0;

            foreach (var product in featuredProducts)
            {

                if (i == 0)
                {
                    var pWindow = new Window("Featured", 1, 16, new List<string> { $"{product.Name,-30}", $"{product.Price.ToString(),-7} SEK", $"Id: {product.Id.ToString()}" });
                }
                if (i == 1)
                {
                    var pWindow = new Window("Featured", 41, 16, new List<string> { $"{product.Name,-30}", $"{product.Price.ToString(),-7} SEK", $"Id: {product.Id.ToString()}" });
                }
                if (i == 2)
                {
                    var pWindow = new Window("Featured", 81, 16, new List<string> { $"{product.Name,-30}", $"{product.Price.ToString(),-7} SEK", $"Id: {product.Id.ToString()}" });
                }
                i++;

            }
        }

        private async Task AddProduct()
        {
            Console.Write("Enter product name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Invalid name");
                return;
            }

            Console.Write("Enter product price: ");
            decimal price;

            bool success = decimal.TryParse(Console.ReadLine(), out price);
            if (success == false)
            {
                Console.WriteLine("Invalid price");
                return;
            }

            Console.Write("Enter product description: ");

            var description = Console.ReadLine();

            if (string.IsNullOrEmpty(description))
            {
                Console.WriteLine("Invalid description");
                return;
            }
            var categoryString = Console.ReadLine();

            if (string.IsNullOrEmpty(categoryString))
            {
                Console.WriteLine("Invalid category");
                return;
            }

            var category = await _WebShop.ProductManager.SearchCategory(categoryString);
            if (category == null)
            {
                return;
            }

            var product = new Product
            {
                Name = name,
                Price = price,
                Description = description,
                Categories = new List<Category>() { category }
            };

            await _WebShop.ProductManager.AddProduct(product);
        }

        private async Task DisplayCategories()
        {
            var categories = await _WebShop.ProductManager.CategoriesToList();

            foreach (var category in categories)
            {
                Console.WriteLine($"{category.Id,-6} {category.Name,-20}");
            }

            int id;
            Console.Write("Enter Id: ");
            var stringId = Console.ReadLine();
            if (string.IsNullOrEmpty(stringId)) return;

            Console.WriteLine("[E] to edit name | [D] to delete | [X] to exit | [P] to view category products");
            while (int.TryParse(stringId, out id))
            {

                var k = Console.ReadKey(true);
                switch (k.Key)
                {
                    case ConsoleKey.E:
                        Console.Write("Enter new name: ");
                        var newName = Console.ReadLine();
                        if (string.IsNullOrEmpty(newName)) continue;

                        await _WebShop.ProductManager.UpdateCategory(id, newName);
                        continue;
                    case ConsoleKey.D:
                        await _WebShop.ProductManager.DeleteCategory(id);
                        continue;
                    case ConsoleKey.P:
                        await DisplayCategoryProducts(id);
                        continue;
                    case ConsoleKey.X:
                        return;
                }
            }
        }

        private async Task DisplayCategoryProducts(int categoryId)
        {
            var categoryProducts = await _WebShop.ProductManager.GetCategoryProducts(categoryId);
            if (categoryProducts == null)
            {
                Console.WriteLine("No products in this category");
                return;
            }

            foreach (var p in categoryProducts)
            {
                Console.WriteLine($"{p.Id,-7}{p.Name,-30}{p.Categories,-30}{p.IsFeatured,-15}{p.Price + " SEK",-10}");
            }

        }

        private async Task DisplayProductDetailsAdmin(int id)
        {
            var product = await DisplayProductDetails(id);
            if (product == null) return;

            Console.WriteLine("\n[N] Edit Name | [P] Edit Price | [D] Edit Description | [F] Toggle Featured | [X] Exit");
            Console.WriteLine("[C]Add category | [R] Remove category");
            string? newName = null;
            decimal? newPrice = null;
            bool? isFeatured = null;
            string? newDescription = null;
            string? newCategory = null;
            string? removeCategory = null;

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.N:
                        Console.Write("Enter new name: ");
                        newName = Console.ReadLine();
                        break;
                    case ConsoleKey.P:
                        Console.Write("Enter new price: ");
                        if (decimal.TryParse(Console.ReadLine(), out var price))
                            newPrice = price;
                        break;
                    case ConsoleKey.D:
                        Console.Write("Enter new description: ");
                        newDescription = Console.ReadLine();
                        break;
                    case ConsoleKey.F:
                        isFeatured = !product.IsFeatured;
                        Console.WriteLine($"Featured status set to {isFeatured.Value}");
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine();
                        Console.WriteLine("Enter category: ");
                        newCategory = Console.ReadLine();
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine();
                        Console.WriteLine("Enter category: ");
                        removeCategory = Console.ReadLine();
                        break;
                    case ConsoleKey.X:
                        // Apply all changes at once
                        if (string.IsNullOrEmpty(newName) && newPrice == null && isFeatured == null && string.IsNullOrEmpty(newDescription) && string.IsNullOrEmpty(newCategory) && string.IsNullOrEmpty(removeCategory)) return;
                        await _WebShop.ProductManager.UpdateProduct(id, newName, newPrice, isFeatured, newDescription, newCategory);
                        await _WebShop.ProductManager.RemoveCategoryFromProduct(id, removeCategory);
                        return;
                }
            }
        }

        private async Task<Product> DisplayProductDetails(int id)
        {
            var product = await _WebShop.ProductManager.GetProduct(id);
            if (product == null) return null;

            Console.Clear();
            Console.WriteLine(new string('=', 110)); // Top border
            Console.WriteLine($"{"Product Details",-50}");
            Console.WriteLine(new string('=', 110));

            Console.WriteLine($"{"ID:",-15} {product.Id}");
            Console.WriteLine($"{"Name:",-15} {product.Name}");
            Console.WriteLine($"{"Categories:",-15} {string.Join(", ", product.Categories.Select(c => c.Name))}");
            Console.WriteLine($"{"Featured:",-15} {product.IsFeatured}");
            Console.WriteLine($"{"Price:",-15} {product.Price} SEK");
            Console.WriteLine(new string('-', 110)); // Separator for the description

            Console.WriteLine("\nDescription:");
            Console.WriteLine($"{product.Description,40}");
            Console.WriteLine(new string('-', 110));
            return product;

        }

        private async Task<int> DisplayProducts(List<Product> products = null, bool isAdmin = false, bool skipUserInput = false)
        {
            if (products == null)
            {
                products = await _WebShop.ProductManager.ProductsToList();
            }

            if (products == null)
            {
                Console.WriteLine("There are no products.");
                return -1;
            }

            UiHelper.ChangeConsoleColors(foreground: ConsoleColor.Black, background: ConsoleColor.White);
            if (isAdmin)
                Console.WriteLine($"{"ID",-6}{"Name",-35}{"Categories",-35}{"Price",-20}{"Copies_sold",-20}");
            else
                Console.WriteLine($"{"ID",-6}{"Name",-35}{"Categories",-35}{"Price",-20}");
            Console.WriteLine(new string('-', 116)); // A line to separate headers from the data
            Console.ResetColor();

            // Display products
            foreach (var p in products)
            {
                if (isAdmin)
                    Console.WriteLine($"{p.Id,-6}{p.Name,-35}{string.Join(',', p.Categories.Select(c => c.Name)),-35}{p.Price,-20}{p.SoldCount,7}");
                else
                    Console.WriteLine($"{p.Id,-6}{p.Name,-35}{string.Join(',', p.Categories.Select(c => c.Name)),-35}{p.Price,-20}");

            }

            if (!skipUserInput)
            {
                return await GetProductIdFromUser();
            }
            return -1;
        }

        private static async Task<int> GetProductIdFromUser()
        {
            // Prompt for product ID input
            Console.Write("Enter product ID: ");

            string productIdString = Console.ReadLine();
            int productId = 0;

            if (!int.TryParse(productIdString, out productId)) return -1;

            return productId;
        }
        private static async Task DisplayLogo()
        {
            Console.SetCursorPosition(100, 0);
            Console.WriteLine(@"
                                     __     ______  _    _   _      ____   _____  _     _____ ____  
                                     \ \   / / __ \| |  | | | |    / __ \ / ____|| |   |  __ \___ \ 
                                      \ \_/ / |  | | |  | | | |   | |  | | (___  | |   | |__) |__) |
                                       \   /| |  | | |  | | | |   | |  | |\___ \ | |   |  ___/|__ < 
                                        | | | |__| | |__| | | |___| |__| |____) || |___| |    ___) |
                                        |_|  \____/ \____/  |______\____/|_____/ |_____|_|   |____/ 
                                                               
                                                          Your Local Game Shop
                                                                                                ");

        }

        private async Task AddCategory()
        {
            Console.Write("Enter category name: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                await _WebShop.ProductManager.CreateCategory(name);
                return;
            }
            Console.WriteLine("Invalid name.");
        }

        private async Task ViewCustomers()
        {
            var users = await _WebShop.UserManager.GetUsers();
            foreach (var u in users)
            {
                Console.WriteLine($"{u.Id,-5}{u.Email,-35}{u.FirstName,-10}{u.LastName,-15}");
            }
            Console.WriteLine("Type user id to view and update user details");


            string? newEmail = null;
            string? newFirstname = null;
            string? newLastname = null;
            string? newPasswordHash = null;
            string? newPhonenumber = null;
            string? userIdString = null;
            int userId = 0;

            userIdString = Console.ReadLine();
            if (string.IsNullOrEmpty(userIdString)) return;
            if (!int.TryParse(userIdString, out userId)) return;
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;


            Console.Clear();
            Console.WriteLine(new string('-', 60)); // Separator line
            Console.WriteLine($"{"User Details",-30}");
            Console.WriteLine(new string('-', 60));

            Console.WriteLine($"{"ID:",-15} {user.Id}");
            Console.WriteLine($"{"Email:",-15} {user.Email}");
            Console.WriteLine($"{"First Name:",-15} {user.FirstName}");
            Console.WriteLine($"{"Last Name:",-15} {user.LastName}");
            Console.WriteLine($"{"Phone Number:",-15} {user.PhoneNumber}");
            Console.WriteLine(new string('-', 60));

            Console.WriteLine($"{"Address Details",-30}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine($"{"City:",-15} {user.City}");
            Console.WriteLine($"{"Postal Code:",-15} {user.PostalCode}");
            Console.WriteLine(new string('-', 60));
            Console.WriteLine("[E] to update email | [N] to update name | [P] to update password | [O] to view customer orders | [X] to exit");
            bool oPressed = false;
            while (true)
            {
                var k = Console.ReadKey(true);
                switch (k.Key)
                {
                    case ConsoleKey.E:
                        newEmail = GetEmail();
                        break;
                    case ConsoleKey.N:
                        (newFirstname, newLastname) = GetName();
                        break;
                    case ConsoleKey.P:
                        newPasswordHash = GetPasswordHash();
                        break;
                    case ConsoleKey.O:
                        if (oPressed == true) continue;
                        await DisplayCustomerOrders(userId);
                        oPressed = true;
                        continue;
                    case ConsoleKey.X:
                        if (userIdString == null && newEmail == null && newFirstname == null && newLastname == null && newPasswordHash == null && newPhonenumber == null) return;

                        await _WebShop.UserManager.UpdateUser(userId, newEmail, newFirstname, newLastname, newPhonenumber, newPasswordHash);

                        return;
                    default:
                        Console.WriteLine("invaild input");
                        Thread.Sleep(1000);
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write(new string(' ', 80));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        continue;


                }
            }
        }

        public async Task DisplayCustomerOrders(int id)
        {

            var userOrders = await _WebShop.OrderManager.OrdersToListAsync(id);
            Console.WriteLine("\n===Orders===========");
            foreach (var o in userOrders)
            {
                Console.WriteLine(new string('-', 100));
                Console.WriteLine($"{o.Id,-5}{o.Total + " SEK",-20}{o.PaymentMethod,-15}{o.Status,-25}{o.OrderDate,-25}");
                Console.WriteLine(new string('-', 100));
                Console.Write("Items: ");
                Console.WriteLine(o.Products.Count());
                Console.WriteLine(string.Join(',', o.Products.Select(p => p.Name)));
                Console.WriteLine(new string('-', 100));

            }

        }

        public void WelcomeUser()
        {
            var text1 = "Welcome to " + _WebShop.Settings.ShopName;
            var text2 = "would you like to sign in or sign up?";
            var text3 = "press S for signup, press L for login ";

            var welcome = new Window(_WebShop.Settings.ShopName, 2, 1, new List<string> { text1, text2, text3 });

        }

        public async Task UserInput()
        {

            var k = Console.ReadKey(true);
            switch (k.Key)
            {
                case ConsoleKey.S:
                    await SignUp();
                    break;
                case ConsoleKey.L:

                     await LogIn();
                    break;
                default:

                    Console.WriteLine("\nInvalid input");
                    Thread.Sleep(1000);
                    Console.Clear();

                    break;
            }


        }

        private async Task<bool> LogIn()
        {
            Console.WriteLine("Please log In");
            var email = GetEmail();
            if (email == null) return false;

            var hashedPassword = GetPasswordHash();
            if (hashedPassword == null) return false;

            var user = await _WebShop.UserManager.LogInUser(new LogInModel
            {
                Email = email,
                Password = hashedPassword
            });



            if (user != null)
            {
                _currentUser = user;
                _IsLoggedIn = true;
                return true;
            }
            return false;

            // Console.ReadLine();
        }







        private async Task SignUp()
        {
            Console.WriteLine("Please sign up");
            try
            {

                var email = GetEmail();
                if (email == null) throw new Exception("Email was null");

                var hashedPassword = GetPasswordHash();
                if (hashedPassword == null) throw new Exception("Password was null");

                var (firstName, lastName) = GetName();
                if (firstName == null || lastName == null) throw new Exception("Name was null");

                var phone = GetPhoneNumber();
                if (phone == null) throw new Exception("Phone number was null");

                var role = GetRole();
                if (role == null) return;

                var user = await _WebShop.UserManager.CreateUser(new Models.User
                {
                    Email = email,
                    Password = hashedPassword,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phone,
                    Role = role,
                    Cart = new Cart()
                });
                if (user == null) return;

                Console.WriteLine("User created");
                Thread.Sleep(1000);
                Console.Clear();
                var userSignUp = await _WebShop.UserManager.LogInUser(new LogInModel { Email = user.Email, Password = user.Password });
                if (userSignUp != null)
                {

                    _currentUser = userSignUp;
                    _IsLoggedIn = true;
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        private string? GetEmail()
        {
            Console.WriteLine("Type exit to exit ");
            Console.Write("Enter your email: ");
            var cursor = Console.GetCursorPosition();




            Console.SetCursorPosition(18, cursor.Top);
            Console.Write(new String(' ', 35));
            Console.SetCursorPosition(18, cursor.Top);
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
            {
                return email;
            }
            if (email.ToLower() == "exit")
            {
                Console.WriteLine("Exiting....             ");
                Thread.Sleep(800);
                Console.WriteLine("Press enter to contine");
                Console.ReadLine();
                Console.Clear();
                return null;
            }
            return null;


        }

        private string? GetPasswordHash()
        {

            Console.Write("Enter your password: ");
            var password = InputHelper.ReadKeysPassword(); ;
            if (!string.IsNullOrWhiteSpace(password) && password.Length > 5)
            {
                var hashedPassword = InputHelper.HashPassword(password);
                return hashedPassword;
            }
            if (password.ToLower() == "exit")
            {
                Console.WriteLine("Exiting....             ");
                Thread.Sleep(800);
                Console.WriteLine("Press enter to contine");
                Console.ReadLine();
                Console.Clear();
                return null;
            }
            Console.WriteLine("Password must be at least 6 characters long. Press Enter to continue");
            Console.WriteLine();
            return null;
        }

        private (string? firstName, string? lastName) GetName()
        {
            Console.Write("Enter your first name and last name: ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name) && name.Split(' ').Length >= 2)
            {
                var parts = name.Split(' ');
                return (parts[0], parts[1]);
            }
            return (null, null);
        }


        private string? GetPhoneNumber()
        {
            Console.Write("Enter your phone number: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 5)
            {
                return phone;
            }
            return null;
        }

        private string? GetRole()
        {
            Console.WriteLine("Available roles");
            Console.WriteLine("[Admin] or [Customer]");
            Console.Write("Enter your role: ");
            var role = Console.ReadLine();
            role.ToLower();
            if (role != "admin" || role != "customer")
            {
                return role;
            }
            return null;
        }

        public async Task LiveSearchWithCancellationAsync()
        {

            var products = await _WebShop.ProductManager.ProductsToList();
            var cts = new CancellationTokenSource();

            await foreach (var search in InputHelper.ReadKeysSearchAsync())
            {
                cts.Cancel(); // Cancel the previous search
                cts = new CancellationTokenSource(); // Create a new token

                try
                {
                    var results = await Task.Run(() =>
                    {
                        return products
                                            .Where(p => p.Description
                                                        .Contains(search, StringComparison.OrdinalIgnoreCase)
                                                        || p.Categories.Any(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                                                        || search.All(s => p.Name.Contains(s, StringComparison.OrdinalIgnoreCase))
                                                   )

                                                .ToList();


                    }, cts.Token);

                    Console.Clear();
                    Console.WriteLine("Press enter when done, note the ID for the product you wish to add to cart");
                    Console.WriteLine($"Search: {search}");
                    Console.WriteLine($"{"ID",-5} {"Name",-40}   {"Price(SEK)",-10} ");
                    foreach (var product in results)
                    {

                        Console.WriteLine($"{product.Id,-5} - {product.Name,-40} {product.Price,-10} ");
                    }



                }
                catch (OperationCanceledException)
                {
                    // Ignore canceled tasks
                }
            }
        }


    }
}
