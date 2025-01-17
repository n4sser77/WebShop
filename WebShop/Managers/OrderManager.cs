using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using WebShop.Models.Interfaces;

namespace WebShop.Managers
{
    public class OrderManager : IOrderManager
    {

        public async Task<Order> CreateOrder(User customer, string paymentMethod)
        {
            try
            {

                using var db = new AppDbContext();

                var customerFromDb = await db.Users.Include(u => u.Cart)
                                   .ThenInclude(c => c.Products)
                                   .FirstOrDefaultAsync(u => u == customer);

                if (customerFromDb == null) throw new Exception("Customer not found");
                if (customerFromDb.Cart == null) throw new Exception("Customer cart is null");
                if (customerFromDb.Cart.Products.Count == 0) throw new Exception("Cart is empty");

                customerFromDb.Cart.Total = customerFromDb.Cart.Products.Sum(p => p.Price);
                var order = new Order
                {
                    Customer = customerFromDb,
                    Products = customerFromDb.Cart.Products,
                    Total = customerFromDb.Cart.Total,
                    OrderDate = DateTime.Now,
                    Status = "Created",
                    PaymentMethod = paymentMethod
                };
                db.Orders.Add(order);
                await db.SaveChangesAsync();

                return order;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Order?> GetOrder(int orderId)
        {
            try
            {
                using var db = new AppDbContext();
                var order = await db.Orders.Include(o => o.Products)
                                          .ThenInclude(p => p.Categories)
                                          .Include(o => o.Customer)
                                          .FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null) throw new Exception("Order not found");
                return order;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Order>?> OrdersToListAsync(int userId)
        {
            try
            {
                using var db = new AppDbContext();

                var userOrders = await db.Orders.Include(o => o.Products)
                                          .Include(o => o.Customer)
                                          .Where(o => o.CustomerId == userId).ToListAsync();
                if (userOrders == null) return null;

                return userOrders;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
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

            try
            {
                using var db = new AppDbContext();
                var order = await db.Orders.Include(o => o.Products)
                                            .Include(o => o.Customer)
                                            .FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null) throw new ArgumentNullException("Order not found");
                if (order.Customer == null) throw new ArgumentNullException("Customer not found");
                if (order.Products == null) throw new ArgumentNullException("Products are empty");
                if (order.Customer.Country == null || order.Customer.City == null || order.Customer.FirstName == null || order.Customer.LastName == null) throw new ArgumentNullException("Customer details empty");

                // Logic to proccess order, e.g. send email to customer
                // e.g. update stock levels
                // e.g. update customer loyalty points
                // e.g. update customer order history
                // e.g  create invoice
                // e.g. create delivery note
                // e.g. create shipping label


                if (order.PaymentMethod == "Invoice")
                {
                    // Logic to create invoice
                    await MyPdfLibrary.PdfGenerator.CreateInvoice(orderId.ToString() + "-Invoice.pdf", order.Customer.FirstName + " " + order.Customer.LastName, order.OrderDate, order.Products);
                    Console.WriteLine("Order created and an invoice is saved locally");

                }

                order.Status = "Processed";

                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;

            }
        }
    }
}
