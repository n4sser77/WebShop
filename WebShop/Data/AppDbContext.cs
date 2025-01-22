using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Models;

namespace WebShop.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=NassorisAspire\\SQLEXPRESS;Database=Webshop2;Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=True");
            //.EnableSensitiveDataLogging(false) // Enables detailed error messages
            //.LogTo(Console.WriteLine);    // Logs queries and errors to the console
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasIndex(x => x.Email).IsUnique();
            // modelBuilder.Entity<DTOs.PopularProductInCityDto>().HasNoKey(); // Since it's not a table



        }

    }
}
