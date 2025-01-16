namespace WebShop.Models.Interfaces
{
    public interface IProductManager
    {

        public Task<List<Product>?> SearchProduct(string searchString);
        public Task AddProduct(Product product);
        public Task UpdateProduct(Product oldProduct, Product newProduct);
        public Task DeleteProduct(int id);
    }
}