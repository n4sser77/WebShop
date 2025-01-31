
using WebShop.DTOs;

namespace WebShop.Models.Interfaces
{
    public interface IProductManager
    {

        public Task<List<Product>?> SearchProduct(string searchString);
        public Task AddProduct(Product product);
        public Task UpdateProduct(int productId, string? newName = null, decimal? newPrice = null, bool? isFeatured = null, string? newDescription = null, string? newCategory = null);
        public Task DeleteProduct(int id);
        public Task<List<Category>> GetPopularCategories();
        public Task<List<Product>> GetTopProductsAsync();
        public Task<List<Product>> GetFeaturedProducts();
        public Task<Product> GetProduct(int productId);
        public Task<List<PopularProductInCityDto>> GetPopularProductInCity();
        Task<Category> SearchCategory(string categoryString);
        Task<List<Category>> CategoriesToList();
        Task UpdateCategory(int id, string newName);
        Task DeleteCategory(int id);
        Task<List<Product>> GetCategoryProducts(int categoryId);
        Task RemoveCategoryFromProduct(int id, string? removeCategory);
        Task CreateCategory(string name);
        Task<List<Product>> ProductsToList();
    }
}