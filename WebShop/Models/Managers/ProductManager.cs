using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class ProductManager : IProductManager
    {

        public async Task<Product> GetProduct(int id)
        {

            try
            {
                using var db = new AppDbContext();
                return db.Products.FirstOrDefault(p => p.Id == id);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task AddProduct(Product product)
        {
            try
            {

                using var db = new AppDbContext();
                db.Products.Add(product);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task UpdateProduct(Product oldProduct, Product newProduct)
        {
            try
            {

                using var db = new AppDbContext();
                var oldProductFromList = db.Products.FirstOrDefault(p => p.Id == oldProduct.Id);
                oldProductFromList = newProduct;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }
        public async Task DeleteProduct(int id)
        {
            try
            {

                using var db = new AppDbContext();
                var p = db.Products.FirstOrDefault(p => p.Id == id);
                db.Products.Remove(p);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task CreateCategory(string category)
        {
            try
            {

                using var db = new AppDbContext();
                db.Categories.Add(new Category { Name = category });
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Category>> CategoriesToList()
        {
            try
            {

                using var db = new AppDbContext();
                var c = db.Categories.ToList();
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
