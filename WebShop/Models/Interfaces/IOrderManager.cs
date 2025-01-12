namespace WebShop.Models.Interfaces
{
    public interface IOrderManager
    {

        public Task CreateOrder(Customer customer);
        public Task ProcessOrder(int OrderId);
    }
}