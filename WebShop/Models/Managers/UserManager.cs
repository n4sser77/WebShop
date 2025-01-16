using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class UserManager : IAdminManager
    {


        public async Task<User?> CreateUser(User user)
        {

            try
            {
                using var db = new AppDbContext();

                var existningUser = await db.Users.FirstOrDefaultAsync(u => u == user);
                if (existningUser != null)
                {
                    Console.WriteLine("User already exists");
                    return null;
                }
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            if (user != null)
            {

                Console.WriteLine("User sucessfully created");
                return user;
            }
            return null;
        }
        public async Task AddAdmin(User user)
        {
            using var db = new AppDbContext();
            user.Role = "Admin";
            await db.AddAsync(user);
            await db.SaveChangesAsync();
        }
        public async Task RemoveAdmin(User user)
        {
            using var db = new AppDbContext();
            user.Role = "User";
            await db.SaveChangesAsync();
        }
        public async Task<User?> LogInUser(LogInModel user)
        {
            using var db = new AppDbContext();
            var userFromDb = await db.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.Email == user.Email);
            if (userFromDb == null)
            {
                Console.WriteLine("\nUser not found, press Enter to continue ");
                Console.ReadLine();
                Console.Clear();
                return null;
            }
            if (userFromDb.Password != user.Password)
            {
                Console.WriteLine("\nInvalid password, press Enter to continue ");
                Console.ReadLine();
                Console.Clear();
                return null;
            }

            if (userFromDb.Role == "Admin")
            {
                Console.WriteLine("Welcome Admin");
                return userFromDb;
            }
            else
            {
                Console.WriteLine("Welcome User");
                return userFromDb;
            }
        }

    }
}
