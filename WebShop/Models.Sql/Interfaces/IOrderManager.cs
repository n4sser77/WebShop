namespace WebShop.Models.Interfaces
{
    public interface IOrderManager
    {

        public Task<Order> CreateOrder(User customer, string paymentMethod);
        public Task ProcessOrder(int OrderId);
    }
}