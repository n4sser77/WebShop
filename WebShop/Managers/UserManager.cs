using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using WebShop.Models.Interfaces;
using WebShop.Models.MongoDb;

namespace WebShop.Managers
{
    public class UserManager : IAdminManager
    {

        public async Task<List<User>> GetUsers()
        {
            try
            {
                using var db = new AppDbContext();
                var users = await db.Users.ToListAsync();
                if (users == null) return new List<User>();
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task UpdateUser(int userId, string? newEmail = null, string? newFirstname = null, string? newLastname = null, string? newphoneNumber = null, string? newPasswordHash = null, string? postalCode = null, string? country = null, string? city = null)
        {
            try
            {
                using var db = new AppDbContext();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return;

                if (!string.IsNullOrEmpty(newFirstname))
                    user.FirstName = newFirstname;

                if (!string.IsNullOrEmpty(newLastname))
                    user.LastName = newLastname;

                if (!string.IsNullOrEmpty(newphoneNumber))
                    user.PhoneNumber = newphoneNumber;
                if (!string.IsNullOrEmpty(newPasswordHash))
                    user.Password = newPasswordHash;

                if (!string.IsNullOrEmpty(newEmail))
                    user.Email = newEmail;

                if (!string.IsNullOrEmpty(country))
                    user.Country = country;

                if (!string.IsNullOrEmpty(city))
                    user.City = city;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task<User?> CreateUser(User user)
        {

            try
            {
                using var db = new AppDbContext();

                var existningUser = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existningUser != null)
                {
                    Console.WriteLine("User already exists");
                    return null;
                }
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                Console.WriteLine("User successfully created");
                await Logger.AddLog(user, "Successfull account creation");
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

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
                await Logger.AddLog(userFromDb, "Attempted login");
                return null;
            }

            if (userFromDb.Role == "Admin")
            {
                Console.WriteLine("Welcome Admin");
                await Logger.AddLog(userFromDb, "Successfull login");
                return userFromDb;
            }
            else
            {
                Console.WriteLine("Welcome User");
                await Logger.AddLog(userFromDb, "Successfull login");
                return userFromDb;
            }


        }

    }
}
