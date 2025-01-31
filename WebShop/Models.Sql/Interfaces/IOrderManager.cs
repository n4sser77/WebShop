
namespace WebShop.Models.Interfaces
{
    public interface IOrderManager
    {

        public Task<Order> CreateOrder(User customer, string paymentMethod);
        public Task<List<Order>?> OrdersToListAsync(int id);
        public Task ProcessOrder(int OrderId);
    }
}