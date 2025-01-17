using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShop.Models
{
    public class LogInModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; } = "";
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public virtual int CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        // must be called before order creation
        public User FillCustomerDetails(string postalCode, string city, string country, string phoneNumber)
        {
            return new User
            {
                Id = this.Id,
                Email = this.Email,
                Password = this.Password,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PhoneNumber = phoneNumber,
                Role = this.Role,
                Cart = this.Cart,
                CartId = this.CartId,
                PostalCode = postalCode,
                City = city,
                Country = country,

            };
        }


    }




}
