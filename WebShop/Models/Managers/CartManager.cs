﻿using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class CartManager : ICartManager
    {

        public async Task<List<Product>> GetUserCart(User user)
        {
            using var db = new AppDbContext();
            var userFromDb = await db.Users.Include(u => u.Cart)
                                           .ThenInclude(c => c.Products)
                                        .FirstOrDefaultAsync(u => u == user);
            if (userFromDb != null)
            {
                var cart = userFromDb.Cart;
                if (cart != null)
                {
                    return cart.Products.ToList();
                }
            }
            return new List<Product>();
        }
        public async Task RemoveFromCart(User user, string id)
        {
            if (id != null && int.TryParse(id, out int idInt))
            {

                try
                {
                    using var db = new AppDbContext();
                    var userFromDb = await db.Users.Include(u => u.Cart).ThenInclude(c => c.Products).FirstOrDefaultAsync(u => u.Id == user.Id);
                    if (userFromDb == null) throw new InvalidOperationException($"User with ID {user.Id} not found.");
                    if (userFromDb.Cart == null) throw new InvalidCastException($"User with ID {user.Id} does not have a cart.");
                    var product = await db.Products.FirstOrDefaultAsync(p => p.Id == idInt);
                    if (product == null) throw new InvalidOperationException($"Product with ID {idInt} not found.");
                    if (userFromDb.Cart.Products.Remove(product))
                    {
                        Console.WriteLine(product.Name + " removed from cart");
                        await db.SaveChangesAsync();
                    }





                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public async Task AddToUserCart(User user, Product product)
        {
            try
            {
                using var db = new AppDbContext();

                // Load the user and their cart from the database
                var userFromDB = await db.Users
                                         .Include(u => u.Cart)
                                         .ThenInclude(c => c.Products)
                                         .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userFromDB == null)
                {
                    throw new InvalidOperationException($"User with ID {user.Id} not found.");
                }

                // Ensure the user has a cart; if not, create one
                if (userFromDB.Cart == null)
                {
                    userFromDB.Cart = new Cart { Products = new List<Product>() };
                    db.Carts.Add(userFromDB.Cart); // Add the new cart to the context
                }


                var productFromDB = await db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (productFromDB == null)
                {
                    throw new InvalidOperationException($"Product with ID {product.Id} not found.");
                }
                userFromDB.Cart.Products.Add(productFromDB);


                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while adding to cart: {e.Message}");
                throw;
            }
        }


        public async Task CheckoutCart(int cartId)
        {
            using var db = new AppDbContext();
            var cart = db.Carts.FirstOrDefault(c => c.Id == cartId);
            if (cart != null)
            {
                cart.IsCheckedOut = true;
                await db.SaveChangesAsync();
            }
        }

    }
}
