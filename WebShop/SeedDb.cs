using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebShop.Data;
using WebShop.Models;

namespace WebShop
{
    internal class SeedDb
    {
        public static async Task SeedWithUsers()
        {
            using var db = new AppDbContext();

            try
            {
                var faker = new Bogus.Faker("en");

                var users = Enumerable.Range(1, 50).Select(_ => new User
                {
                    Email = faker.Internet.Email(),
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Country = faker.Address.Country(),
                    City = faker.Address.City(),
                    Password = faker.Internet.Password(),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                    PostalCode = faker.Address.ZipCode(),
                    Role = "Customer",
                    Cart = new Cart(),
                });

                await db.Users.AddRangeAsync(users);
                await db.SaveChangesAsync();
                Console.WriteLine("Successfully seeded users!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error seeding users: {e.Message}");
                throw;
            }
        }
        public static async Task SeedWithOrders()
        {
            using var db = new AppDbContext();

            try
            {
                var faker = new Bogus.Faker("en");

                var users = db.Users.ToList();
                var products = db.Products.ToList();


                if (!users.Any() || !products.Any())
                {
                    Console.WriteLine("No users or products found. Ensure the database is seeded with these first.");
                    return;
                }

                var orders = Enumerable.Range(1, 100).Select(_ =>
                {

                    var randomUser = faker.PickRandom(users);


                    var randomProducts = faker.PickRandom(products, faker.Random.Int(1, 5)).ToList();

                    return new Order
                    {
                        CustomerId = randomUser.Id,
                        Customer = randomUser,
                        OrderDate = faker.Date.Past(1), // Random date in the past year
                        Status = faker.PickRandom(new[] { "Pending", "Completed", "Cancelled" }),
                        PaymentMethod = faker.PickRandom(new[] { "Invoice", "PayPal" }),
                        Total = randomProducts.Sum(p => p.Price), // Assuming Product has a Price property
                        Products = randomProducts,
                        
                    };
                }).ToList();

                // Add orders to the database
                await db.Orders.AddRangeAsync(orders);
                await db.SaveChangesAsync();

                Console.WriteLine("Successfully seeded orders!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error seeding orders: {e.Message}");
                throw;
            }
        }

        public static async Task SeedDatabase()
        {
            using var db = new AppDbContext();

            try
            {


                // Ensure the required categories exist
                var categoryNames = new[] { "Survivor horror", "Open world", "RPG", "Shooter" };
                var categories = db.Categories
                    .Where(c => categoryNames.Contains(c.Name))
                    .ToList();

                if (categories.Count != categoryNames.Length)
                {
                    throw new Exception("Some required categories are missing in the database. Please add them first.");
                }

                // Map category names to their objects for easy lookup
                var categoryMap = categories.ToDictionary(c => c.Name, c => c);

                // Create a list of products with many-to-many relationships
                var products = new List<Product>
            {
                new Product
                {
                    Name = "The Last of Us 2",
                    Description = "Experience the winner of over 300 Game of the Year awards now with an array of technical enhancements that make The Last of Us Part II Remastered the definitive way to play Ellie and Abby’s critically acclaimed story.",
                    Price = 899,
                    Categories = new List<Category> { categoryMap["Survivor horror"] }
                },
                new Product
                {
                    Name = "The Last of Us: Left Behind",
                    Description = "The Last of Us: Left Behind combines themes of survival, loyalty, and love with tense, survival-action gameplay in this critically acclaimed chapter to the best-selling game, The Last of Us.",
                    Price = 299,
                    Categories = new List<Category> { categoryMap["Survivor horror"] }
                },
                new Product
                {
                    Name = "Grand Theft Auto 5",
                    Description = "When a young street hustler, a retired bank robber and a terrifying psychopath find themselves entangled with some of the most frightening and deranged elements of the criminal underworld, the U.S. government and the entertainment industry, they must pull off a series of dangerous heists to survive in a ruthless city in which they can trust nobody, least of all each other.",
                    Price = 499,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "Red Dead Redemption 2",
                    Description = "America, 1899. The end of the wild west era has begun as lawmen hunt down the last remaining outlaw gangs. Those who will not surrender or succumb are hunted to extinction.",
                    Price = 699,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },

                new Product
                {
                    Name = "Horizon Zero Dawn",
                    Description = "Experience Aloy's legendary quest to unravel the mysteries of a world ruled by deadly Machines.",
                    Price = 599,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },
                new Product
                {
                    Name = "Horizon Forbidden West",
                    Description = "Join Aloy as she braves the Forbidden West – a majestic but dangerous frontier that conceals mysterious new threats.",
                    Price = 699,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },
                new Product
                {
                    Name = "Call of Duty: Black Ops 6",
                    Description = "Experience the next generation of Black Ops warfare with intense, action-packed multiplayer and an engaging campaign.",
                    Price = 799,
                    Categories = new List<Category> { categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "The Witcher 3: Wild Hunt",
                    Description = "As Geralt of Rivia, a monster hunter for hire, explore a war-torn world and track down the Child of Prophecy.",
                    Price = 499,
                    Categories = new List<Category> { categoryMap["RPG"], categoryMap["Open world"] }
                },
                new Product
                {
                    Name = "Assassin's Creed: Origins",
                    Description = "Uncover the mysteries of Ancient Egypt as you embark on a quest to form the Brotherhood.",
                    Price = 599,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },
                new Product
                {
                    Name = "Cyberpunk 2077",
                    Description = "Become a cyberpunk mercenary in the neon-lit streets of Night City, where your choices shape the story.",
                    Price = 699,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },
                new Product
                {
                    Name = "Assassin's Creed: Valhalla",
                    Description = "Lead epic Viking raids against Saxon troops and fortresses as you carve your place in history.",
                    Price = 699,
                    Categories = new List<Category> { categoryMap["Open world"], categoryMap["RPG"] }
                },
                new Product
                {
                    Name = "Sniper Elite",
                    Description = "Play as an elite sniper in a deadly warzone, using stealth and precision to take down your enemies.",
                    Price = 499,
                    Categories = new List<Category> { categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "Call of Duty: Modern Warfare 2",
                    Description = "Engage in high-stakes operations and intense multiplayer action in the latest installment of Modern Warfare.",
                    Price = 899,
                    Categories = new List<Category> { categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "Battlefield 1",
                    Description = "Experience the dawn of all-out warfare during World War I with epic battles and immersive environments.",
                    Price = 499,
                    Categories = new List<Category> { categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "Battlefield 2042",
                    Description = "Adapt and overcome in a massive-scale warfare experience set in a near-future world on the brink of chaos.",
                    Price = 899,
                    Categories = new List<Category> { categoryMap["Shooter"] }
                },
                new Product
                {
                    Name = "Days Gone",
                    Description = "Ride and survive in a post-apocalyptic open world, where danger lurks around every corner.",
                    Price = 599,
                    Categories = new List<Category> { categoryMap["Survivor horror"], categoryMap["Open world"] }
                },
                new Product
                {
                    Name = "Outlast",
                    Description = "Delve into the horrors of Mount Massive Asylum and uncover the secrets lurking within.",
                    Price = 399,
                    Categories = new List<Category> { categoryMap["Survivor horror"] }
                },
                new Product
                {
                    Name = "Outlast 2",
                    Description = "Journey through the terrifying deserts of Arizona as you uncover the truth behind a sinister cult.",
                    Price = 499,
                    Categories = new List<Category> { categoryMap["Survivor horror"] }
                }
            };





                // Add products and their category relationships only if they don't already exist
                foreach (var product in products)
                {
                    if (!db.Products.Any(p => p.Name == product.Name))
                    {
                        db.Products.Add(product);
                    }
                }

                // Save changes to the database
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
