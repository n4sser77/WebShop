using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models;
using WebShop.Models.Managers;
using WindowGUI;


namespace WebShop
{
    public class Shop
    {

        private Models.WebShop _WebShop { get; set; }
        private bool _IsLoggedIn { get; set; } = false;
        private User _currentUser { get; set; }

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

            while (true)
            {
                if (_IsLoggedIn)
                {
                    Console.Clear();
                    var welcomeText = "Welcome " + _currentUser.Role + "!";
                    var loggedInText = "Logged in as " + _currentUser.FirstName;

                    var window1 = new Window("", 2, 1, new List<string> { welcomeText, loggedInText });
                    var toolbarWindow = new Window("Admin tools", 42, 1, new List<string> { "Press  D to add a category", "Press L to list categories" });


                    var k = Console.ReadKey(true);
                    switch (k.Key)
                    {
                        case ConsoleKey.D:
                            Console.Write("Enter category name: ");

                            await _WebShop.ProductManager.CreateCategory(Console.ReadLine());

                            break;
                        case ConsoleKey.L:
                            var categories = await _WebShop.ProductManager.CategoriesToList();

                            foreach (var category in categories)
                            {
                                Console.WriteLine(category);
                            }
                            break;
                        default:
                            Console.WriteLine("Invaild key pressed");
                            break;
                    }


                    Console.ReadLine();

                }
                else
                {
                    WelcomeUser();
                    await UserInput();
                    continue;
                }
            }
        }

        public async Task WelcomeUser()
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

            _currentUser = await _WebShop.UserManager.LogInUser(new LogInModel
            {
                Email = email,
                Password = hashedPassword
            });

            _IsLoggedIn = true;
            // Console.ReadLine();
        }

        private async Task SignUp()
        {
            Console.WriteLine("Please sign up");

            var email = GetEmail();
            if (email == null) return;

            var hashedPassword = GetPasswordHash();
            if (hashedPassword == null) return;

            var (firstName, lastName) = GetName();
            if (firstName == null || lastName == null) return;

            var address = GetAddress();
            if (address == null) return;

            var phone = GetPhoneNumber();
            if (phone == null) return;

            var role = GetRole();
            if (role == null) return;

            var user = await _WebShop.UserManager.CreateUser(new Models.User
            {
                Email = email,
                Password = hashedPassword,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                Role = role
            });


            Console.WriteLine("User created");
            Thread.Sleep(1000);
            Console.Clear();
            _currentUser = await _WebShop.UserManager.LogInUser(new LogInModel { Email = user.Email, Password = user.Password });
        }

        private string GetEmail()
        {
            Console.Write("Enter your email: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
            {
                return email;
            }
            Console.WriteLine("Invalid email.");
            return null;
        }

        private string GetPasswordHash()
        {
            Console.Write("Enter your password: ");
            var password = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(password) && password.Length > 5)
            {
                var hashedPassword = Helpers.HashPassword(password);
                return hashedPassword;
            }
            Console.WriteLine("Password must be at least 6 characters long.");
            return null;
        }

        private (string firstName, string lastName) GetName()
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

        private string GetAddress()
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

        private string GetPhoneNumber()
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

        private string GetRole()
        {
            Console.Write("Enter your role: ");
            var role = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(role) && role.Length > 2)
            {
                return role;
            }
            Console.WriteLine("Invalid role.");
            return null;
        }

    }
}
