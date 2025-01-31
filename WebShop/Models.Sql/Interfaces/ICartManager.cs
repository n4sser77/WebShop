namespace WebShop.Models.Interfaces
{
    public interface ICartManager
    {
        Task AddToUserCart(User currentUser, Product product);

        // public Task<Cart> CreateCart(Customer customer);
        public Task<bool> CheckoutCart(int CartId);
        Task<Cart> GetUserCart(User? currentUser);
        Task RemoveFromCart(User currentUser, string idToRemove);
        Task ClearCart(int userCartID);
    }
}