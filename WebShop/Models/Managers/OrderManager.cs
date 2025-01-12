using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class OrderManager : IOrderManager
    {

        public async Task CreateOrder(Customer customer)
        {
            using var db = new AppDbContext();
            var order = new Order
            {
                Customer = customer,
                Products = customer.Cart.Products,
                Total = customer.Cart.Total,
                OrderDate = DateTime.Now
            };
            db.Orders.Add(order);
            await db.SaveChangesAsync();
        }

        public async Task ProcessOrder(int orderId)
        {
            // Logic to proccess order, e.g. send email to customer
            // e.g. update stock levels
            // e.g. update customer loyalty points
            // e.g. update customer order history
            // e.g  create invoice
            // e.g. create delivery note
            // e.g. create shipping label

        }
    }
}
