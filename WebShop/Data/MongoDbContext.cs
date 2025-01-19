using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using MongoDB.EntityFrameworkCore.Extensions;
using WebShop.Models.MongoDb;
using WebShop.Models;

namespace WebShop.Data
{
    internal class MongoDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string connectionUri = "mongodb+srv://nasseralasbahi:3SJMKGyFfyMfdEdb@cluster0.djk6cr5.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

            var client = new MongoDB.Driver.MongoClient(connectionUri);
            optionsBuilder.UseMongoDB(client, "WebShop");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Log>().ToCollection("Logs");
            ;
        }
    }
}
