using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebShop.Data;
using WebShop.Managers;
using WebShop.Models;
using WindowGUI;


namespace WebShop
{
    public class Shop
    {

        private Models.WebShop _WebShop { get; set; }
        private bool _IsLoggedIn { get; set; } = false;
        private User _currentUser { get; set; }
        private bool isInDev = true;

        public Shop(string shopName)
        {
            CartManager CartManager = new CartManager();
            ProductManager ProductManager = new ProductManager();
            OrderManager OrderManager = new OrderManager();
            UserManager UserManager = new UserManager();
            WebShopSettings Settings = new WebShopSettings
            {
                ShopName = shopName
            };
            _WebShop = new Models.WebShop(Settings, ProductManager, UserManager, CartManager, OrderManager);
        }



        public async Task Run()
        {
            if (isInDev)
            {
                using var db = new AppDbContext();

                var admin = db.Users.FirstOrDefault(u => u.FirstName == "Anders");

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
                    var welcomeText = "Welcome " + _currentUser.Role + "!";
                    var loggedInText = "Logged in as " + _currentUser.FirstName;
                    var loggedInText1 = "Press X to sign out " + _currentUser.FirstName;

                    var window1 = new Window("", 2, 1, new List<string> { welcomeText, loggedInText });
                    var toolbarWindow = new Window("Admin tools", 2, 6, new List<string> { "Press  D to add a category", "Press L to list categories", " ", " ", " " });
                    var toolbar1Window = new Window("Product tools", 35, 6, new List<string> { "Press  P to add a product", "Press A to list products",
                        "Press F to make three random products featured ", "Or E to manually add one", "Press H to list all featured products" });

                    Console.WriteLine("\nPress X to sign out");


                    var k = Console.ReadKey(true);
                    switch (k.Key)
                    {
                        case ConsoleKey.X:
                            Console.Clear();
                            _IsLoggedIn = false;
                            _currentUser = null;
                            continue;
                        case ConsoleKey.H:
                            var feauteredProducts = await _WebShop.ProductManager.GetFeaturedProducts();
                            foreach (var fP in feauteredProducts)
                            {
                                Console.WriteLine(fP.Name);
                            }
                            break;
                        case ConsoleKey.F:
                            var p = await _WebShop.ProductManager.AddRndToFeautered();
                            if (p != null)
                            {
                                Console.WriteLine(p.Name + " added to featured products");
                            }
                            Console.WriteLine("Pr");
                            break;
                        case ConsoleKey.A:
                            await DisplayProducts();
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
                            Console.WriteLine("Invaild key pressed");
                            break;
                    }


                    Console.ReadKey();

                }
                else if (_IsLoggedIn && _currentUser.Role == "Customer")
                {
                    Console.Clear();
                    var welcomeText = "Welcome " + _currentUser.FirstName + "!";


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
                            break;
                        case ConsoleKey.H:
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

                            break;
                        case ConsoleKey.L:
                            var products = await _WebShop.ProductManager.ProductsToList();
                            if (products == null) return;
                            Console.WriteLine($"{"ID",-7}{"Name",-30}{"Price",-7} SEK");
                            foreach (var p in products)
                            {
                                Console.WriteLine($"{p.Id,-7}{p.Name,-30}{p.Price,-7} SEK");
                            }
                            break;
                        case ConsoleKey.S:
                            Console.Write("Enter product name: ");
                            await LiveSearchWithCancellationAsync();

                            Console.WriteLine("Press enter again to continue");
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


                            break;
                        case ConsoleKey.T:
                            Console.Write("Enter product ID");
                            var idString = Console.ReadLine();
                            if (int.TryParse(idString, out int result))
                            {
                                await DisplayProductDetails(result);
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid input");
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

            async Task AddCategory()
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

            async Task DisplayCategories()
            {
                var categories = await _WebShop.ProductManager.CategoriesToList();

                foreach (var category in categories)
                {
                    Console.WriteLine(category.Name);
                }
            }
            async Task DisplayProducts()
            {
                var products = await _WebShop.ProductManager.ProductsToList();

                foreach (var p in products)
                {
                    Console.WriteLine(p.Name);
                }
            }
            async Task DisplayProductDetails(int id)
            {
                var product = await _WebShop.ProductManager.GetProduct(id);
                if (product == null) return;
                Console.Clear();
                Console.WriteLine($"{"ID",-7}{"Name",-30}{"Price",10}");
                Console.WriteLine(new string('-', 50)); // Separator line
                Console.WriteLine($"{product.Id,-7}{product.Name,-30}{product.Price,10} SEK");
                Console.WriteLine("\nDescription:");
                Console.WriteLine($"{product.Description,30}");

            }

            async Task AddProduct()
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

            async Task DisplayFeatured()
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

            async Task DisplayCart()
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

            async Task RenderCartMode()
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("[C] to Checkout");
                Console.ResetColor();
                Console.WriteLine("--Cart------------------------------------------------------------------------------------------------------------------");
                var cart = await _WebShop.CartManager.GetUserCart(_currentUser);
                if (cart == null) return;

                Console.WriteLine($"{"Id",-6}{"Name",-30}{"Categories",-25}{"Price",-7}");
                foreach (var item in cart.Products)
                {
                    Console.WriteLine($"{$"[{item.Id}]",-6}{item.Name,-30}{string.Join(',', item.Categories.Select(c => c.Name)),-25}{item.Price,-7} SEK");
                }

                var total = cart.Products.Sum(p => p.Price);
                Console.WriteLine($"\n{"Taxes",66}");
                var taxes = (float)total * 0.25;
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

                        await _WebShop.UserManager.UpdateUser(customer);
                        Console.WriteLine("Please choose payment method");
                        Console.WriteLine("Invoice");
                        Console.Write(": ");
                        var chosenMethod = Console.ReadLine();
                        if (string.IsNullOrEmpty(chosenMethod)) return;

                        if (customer == null) return;

                        var order = await _WebShop.OrderManager.CreateOrder(customer, chosenMethod);

                        await _WebShop.OrderManager.ProcessOrder(order.Id);

                        await _WebShop.CartManager.ClearCart(checkoutCart.Id);
                    }

                }



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

        private async Task LogIn()
        {
            Console.WriteLine("Please log In");
            var email = GetEmail();
            if (email == null) return;

            var hashedPassword = GetPasswordHash();
            if (hashedPassword == null) return;

            var user = await _WebShop.UserManager.LogInUser(new LogInModel
            {
                Email = email,
                Password = hashedPassword
            });
            if (user != null)
            {
                _currentUser = user;
                _IsLoggedIn = true;
            }

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

                var address = GetAddress();
                if (address == null) throw new Exception("Adress was null");

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
            while (true)
            {



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
                Console.WriteLine("Invalid email.");
            }

        }

        private string? GetPasswordHash()
        {

            Console.Write("Enter your password: ");
            var password = Helpers.ReadKeysPassword(); ;
            if (!string.IsNullOrWhiteSpace(password) && password.Length > 5)
            {
                var hashedPassword = Helpers.HashPassword(password);
                return hashedPassword;
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
            Console.WriteLine("Invalid name.");
            return (null, null);
        }

        private string? GetAddress()
        {
            Console.Write("Enter your address: ");
            var address = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(address) && address.Length > 5)
            {
                return address;
            }
            Console.WriteLine("Invalid address.");
            return null;
        }

        private string? GetPhoneNumber()
        {
            Console.Write("Enter your phone number: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 5)
            {
                return phone;
            }
            Console.WriteLine("Invalid phone number.");
            return null;
        }

        private string? GetRole()
        {
            Console.WriteLine("Available roles");
            Console.WriteLine("[Admin] or [Customer]");
            Console.Write("Enter your role: ");
            var role = Console.ReadLine();
            if (role != "Admin" || role != "Customer")
            {
                return role;
            }
            Console.WriteLine("Invalid role.");
            return null;
        }

        public async Task LiveSearchWithCancellationAsync()
        {

            var products = await _WebShop.ProductManager.ProductsToList();
            var cts = new CancellationTokenSource();

            await foreach (var search in Helpers.ReadKeysSearchAsync())
            {
                cts.Cancel(); // Cancel the previous search
                cts = new CancellationTokenSource(); // Create a new token

                try
                {
                    var results = await Task.Run(() =>
                    {
                        return products
                            .Where(p => p.Name.Trim().Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
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
