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

            //await ResetCartsAndIdentityAsync();
            await app.Run();

            // DB PRODUCT SEEDING
            //await SeedDb.SeedDatabase();
            //Console.WriteLine("Database seeded sucessfully  ");
            //Console.WriteLine();

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

    }
}
