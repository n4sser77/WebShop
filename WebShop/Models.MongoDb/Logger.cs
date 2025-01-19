using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Data;
using WebShop.Models;

namespace WebShop.Models.MongoDb
{
    internal class Logger
    {
        public static async Task AddLog(User user, string action)
        {
            try
            {

                if (user == null) throw new ArgumentNullException("User is null");
                using var mongoDbContext = new MongoDbContext();


                var log = new Log
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRole = user.Role,
                    Date = DateTime.Now,
                    Action = action
                };
                await mongoDbContext.AddAsync(log);
                await mongoDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


    }

    internal class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)] // Ensures the Guid is stored as a string in MongoDB
        public ObjectId Id { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }

    }



}