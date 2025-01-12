using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models.Interfaces;

namespace WebShop.Models.Managers
{
    public class ProductManager : IProductManager
    {

        public async Task<Product?> GetProduct(int id)
        {

            try
            {
                using var db = new AppDbContext();
                var p = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (p != null)
                {
                    return p;
                }
                return null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task AddProduct(Product product)
        {
            if (product == null)
            {
                return;
            }
            try
            {

                using var db = new AppDbContext();
                await db.Products.AddAsync(product);
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
                var oldProductFromList = await db.Products.FirstOrDefaultAsync(p => p.Id == oldProduct.Id);
                if (oldProductFromList == null)
                {
                    return;
                }
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
                var p = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (p == null)
                {
                    return;
                }
                db.Products.Remove(p);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Product>?> ProductsToList()
        {
            try
            {
                using var db = new AppDbContext();
                var p = await db.Products.ToListAsync();
                if (p != null)
                {
                    return p;
                }
                return null;
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
                await db.Categories.AddAsync(new Category { Name = category });
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
                var c = await db.Categories.ToListAsync();
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Category?> SearchCategory(string category)
        {
            if (category == null)
            {
                return null;
            }
            try
            {
                using var db = new AppDbContext();
                var c = await db.Categories.FirstOrDefaultAsync(c => c.Name == category);
                if (c != null)
                {
                    return c;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
