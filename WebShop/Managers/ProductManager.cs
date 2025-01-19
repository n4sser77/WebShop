using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using WebShop.Models.Interfaces;

namespace WebShop.Managers
{
    public class ProductManager : IProductManager
    {

        public async Task<List<Product>?> SearchProduct(string searchQuery)
        {

            try
            {
                using var db = new AppDbContext();
                var p = await db.Products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToListAsync();
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

        public async Task<Product?> GetProduct(int id)
        {
            try
            {
                using var db = new AppDbContext();
                var p = await db.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);
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
        public async Task UpdateProduct(int productId, string? newName = null, decimal? newPrice = null, bool? isFeatured = null, string? newDescription = null)
        {
            try
            {
                using var db = new AppDbContext();
                var product = await db.Products.FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null) return;

                // Update properties only if new values are provided
                if (!string.IsNullOrEmpty(newName))
                    product.Name = newName;

                if (newPrice.HasValue)
                    product.Price = newPrice.Value;

                if (isFeatured.HasValue)
                    product.IsFeatured = isFeatured.Value;

                if (!string.IsNullOrEmpty(newDescription))
                    product.Description = newDescription;

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
                p.IsDeleted = true;
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
                var p = await db.Products.Include(p => p.Categories).ToListAsync();
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
        public async Task<Product?> AddRndToFeautered()
        {
            try
            {
                using var db = new AppDbContext();
                var products = await db.Products.ToListAsync();
                var featuredProducts = products.Where(p => p.IsFeatured == true).ToList();
                if (featuredProducts.Count < 3)
                {
                    var random = new Random();
                    var randomProduct = products[random.Next(0, products.Count)];
                    randomProduct.IsFeatured = true;

                    await db.SaveChangesAsync();
                    return randomProduct;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task AddToFeautered(Product product)
        {
            if (product == null)
            {
                return;
            }
            try
            {
                using var db = new AppDbContext();
                var products = await db.Products.ToListAsync();
                var featuredProducts = products.Where(p => p.IsFeatured == true).ToList();
                if (featuredProducts.Count < 3)
                {
                    product.IsFeatured = true;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        public async Task<List<Product>> GetFeaturedProducts()
        {
            using var db = new AppDbContext();

            var featuredProducts = await db.Products.Where(p => p.IsFeatured == true).ToListAsync();

            if (featuredProducts.Count > 0)
            {
                return featuredProducts;
            }
            return new List<Product>();
        }
    }
}
