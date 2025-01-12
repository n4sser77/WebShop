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
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }

        public virtual int CartId { get; set; }
        public virtual Cart? Cart { get; set; }

        public Customer ToCustomer(string shippingAdress, string billingAdress, string postalCode, string city, string country)
        {
            return new Customer
            {
                Email = this.Email,
                Password = this.Password,
                FirstName = this.FirstName,
                LastName = this.LastName,
                PhoneNumber = this.PhoneNumber,
                Role = this.Role,
                Cart = this.Cart,
                CartId = this.CartId,
                ShippingAddress = shippingAdress,
                BillingAddress = billingAdress,
                PostalCode = postalCode,
                City = city,
                Country = country,

            };
        }


    }

    public class Customer : User
    {
        public required string ShippingAddress { get; set; }
        public required string BillingAddress { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }


}
