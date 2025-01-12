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
            await app.Run();

            // DB PRODUCT SEEDING
            //await SeedDb.SeedDatabase();
            //Console.WriteLine("Database seeded sucessfully  ");
            //Console.WriteLine();

        }
    }
}
