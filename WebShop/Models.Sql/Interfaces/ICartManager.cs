namespace WebShop.Models.Interfaces
{
    public interface ICartManager
    {

        // public Task<Cart> CreateCart(Customer customer);
        public Task<bool> CheckoutCart(int CartId);
    }
}