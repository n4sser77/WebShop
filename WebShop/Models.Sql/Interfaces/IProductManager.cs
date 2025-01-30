namespace WebShop.Models.Interfaces
{
    public interface IProductManager
    {

        public Task<List<Product>?> SearchProduct(string searchString);
        public Task AddProduct(Product product);
        public Task UpdateProduct(int productId, string? newName = null, decimal? newPrice = null, bool? isFeatured = null, string? newDescription = null, string? newCategory = null);
        public Task DeleteProduct(int id);
    }
}