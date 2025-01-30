using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using WebShop.Data;
using WebShop.Models;

namespace WebShop
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Shop app = new Shop("Your console game shop");

            // fuuuck I accedently ran this
            // await ResetCartsAndIdentityAsync();

            // to fix the above mistake
            // await RecreateCartsForUsersAsync();



            // DB PRODUCT SEEDING
            //await SeedDb.SeedDatabase();
            //Console.WriteLine("Database seeded sucessfully  ");
            //Console.WriteLine("Press enter to continue");
            //Console.ReadLine();
            // Seeding with bogus
            //await SeedDb.SeedWithUsers();
            //await SeedDb.SeedWithOrders();
            //Console.WriteLine("Orders seeded, press enter to contiune");
            //Console.ReadLine();
            // already excuted but can be again to seed with more random users and orders. thanks to bogus.


            await app.Run();
        }

        public static async Task ResetCartsAndIdentityAsync()
        {
            try
            {
                using var db = new AppDbContext();

                // Step 1: Delete all carts and their related products
                var carts = await db.Carts.Include(c => c.Products).ToListAsync();

                foreach (var cart in carts)
                {
                    db.Carts.Remove(cart); // Remove the cart and related products
                }

                // Step 2: Reset the identity column of the Carts table (SQL Server example)
                await db.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Carts', RESEED, 0);");

                // Step 3: Save changes
                await db.SaveChangesAsync();

                Console.WriteLine("All carts have been deleted, and the ID counter has been reset.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while resetting carts: {e.Message}");
                throw;
            }
        }

        public static async Task RecreateCartsForUsersAsync()
        {
            try
            {
                using var db = new AppDbContext();

                // Get all users who currently have no cart
                var usersWithoutCarts = await db.Users.ToListAsync();

                // Recreate a cart for each user
                foreach (var user in usersWithoutCarts)
                {
                    var newCart = new Cart
                    {

                        Products = new List<Product>() // Empty cart
                    };
                    user.Cart = newCart;




                }

                await db.SaveChangesAsync();

                Console.WriteLine("Carts have been recreated for users without carts.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while recreating carts: {e.Message}");
                throw;
            }
        }


    }
}
