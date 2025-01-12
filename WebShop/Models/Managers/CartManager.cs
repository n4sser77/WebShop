using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class CartManager : ICartManager
    {

        //public async Task<Cart> CreateCart(Customer customer)
        //{
        //    using var db = new AppDbContext();
        //    customer.Cart = new Cart();

        //    db.Carts.Add(customer.Cart);
        //    await db.SaveChangesAsync();
        //    return customer.Cart;
        //}

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
