using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.DTOs;
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
        public async Task RemoveCategoryFromProduct(int productId, string? categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) return;

            try
            {
                using var db = new AppDbContext();
                var category = await db.Categories.FirstOrDefaultAsync(c => c.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));
                if (category == null) return;
                var product = await db.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == productId);

                product.Categories.Remove(category);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task UpdateProduct(int productId, string? newName = null, decimal? newPrice = null, bool? isFeatured = null, string? newDescription = null, string? newCategory = null)
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

                if (!string.IsNullOrEmpty(newCategory))
                {
                    var existingCategory = db.Categories.FirstOrDefault(c => c.Name.ToUpper().Contains(newCategory.ToUpper()));
                    if (existingCategory == null)
                    {
                        var c = new Category { Name = newCategory };
                        await db.Categories.AddAsync(c);
                        product.Categories.Add(c);
                    }
                    else
                    {
                        product.Categories.Add(existingCategory);
                    }

                }

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

        public async Task UpdateCategory(int id, string newName)
        {
            try
            {
                using var db = new AppDbContext();
                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) throw new NullReferenceException("Category not found");
                category.Name = newName;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public async Task DeleteCategory(int id)
        {
            try
            {
                using var db = new AppDbContext();

                var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) throw new NullReferenceException("Category not found");
                var catogoryProducts = db.Products.Where(p => p.Categories.Contains(category));
                if (catogoryProducts != null)
                {
                    foreach (var p in catogoryProducts)
                    {
                        p.Categories.Remove(category);
                    }
                }

                db.Remove(category);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Product>?> GetCategoryProducts(int id)
        {
            try
            {
                using var db = new AppDbContext();
                var category = db.Categories.FirstOrDefault(c => c.Id == id);
                if (category == null) throw new NullReferenceException("Category was not found");
                var categoryProducts = await db.Products.Where(p => p.Categories.Contains(category)).ToListAsync();

                if (categoryProducts == null) return null;

                return categoryProducts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Product>> GetTopProductsAsync()
        {
            try
            {
                using var db = new AppDbContext();
                var query = """
                            SELECT p.Id, p.Name, p.Price, p.Description,p.IsFeatured, p.IsDeleted, COUNT(op.ProductsId) AS SoldCount
                            FROM dbo.Products p
                            INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                            INNER JOIN CategoryProduct cp ON cp.ProductsId = p.Id
                            INNER JOIN Categories c ON c.Id = cp.CategoriesId
                            GROUP BY p.Id, p.Name, p.Price, p.Description, p.IsFeatured, p.IsDeleted
                            ORDER BY SoldCount DESC
                            """;

                var topProducts = await db.Products
                    .FromSqlRaw(query)
                    .ToListAsync();

                topProducts.ForEach(p => p.Categories = db.Categories.Where(c => c.Products.Any(categoryProduct => categoryProduct.Id == p.Id)).ToList());

                return topProducts;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<DTOs.PopularProductInCityDto>> GetPopularProductInCity()
        {
            try
            {
                using var db = new AppDbContext();
                var query1 = """
                            SELECT p.Id, p.Name, p.Price, p.Description,p.IsFeatured, p.IsDeleted, u.City, u.Country,COUNT(op.ProductsId) AS SoldCount
                            FROM dbo.Products p
                            INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                            INNER JOIN Orders o ON op.OrdersId = o.Id
                            INNER JOIN Users u ON o.CustomerId = u.Id
                            GROUP BY u.Country, u.City,p.Id, p.Name, p.Price, p.Description, p.IsFeatured, p.IsDeleted
                            
                            """;

                var query2 = """
                             SELECT 
                                 u.Country,
                                 p.Id,
                             	p.Name,
                                 p.Price,
                                 p.Description,
                                 p.IsDeleted,
                                 p.IsFeatured,
                                 COUNT(op.OrdersId) AS SoldCount
                             FROM dbo.Products p
                             INNER JOIN OrderProduct op ON p.Id = op.ProductsId
                             INNER JOIN Orders o ON op.OrdersId = o.Id
                             INNER JOIN Users u ON o.CustomerId = u.Id
                             GROUP BY u.Country,p.Id , p.Name, p.Price, p.Description, p.Isdeleted, p.IsFeatured
                             HAVING COUNT( op.OrdersId) > 0
                             ORDER BY u.Country, SoldCount DESC;
                             """;



                var popularProducts = await db.Database.SqlQueryRaw<PopularProductInCityDto>(query2).ToListAsync();

                return popularProducts.Count > 0
                    ? popularProducts
                    : new List<PopularProductInCityDto>();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Category>> GetPopularCategories()
        {
            try
            {
                using var db = new AppDbContext();

                var popularCategories = await db.Categories
                                                .Include(c => c.Products)
                                                .OrderByDescending(c => c.Products.Sum(p => p.SoldCount)).Take(2).ToListAsync();

                popularCategories.ForEach(c =>
                {
                    foreach (var item in c.Products)
                    {

                        item.SoldCount = db.Orders.Count(o => o.Products.Contains(item));
                    }
                });

                return popularCategories;
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
