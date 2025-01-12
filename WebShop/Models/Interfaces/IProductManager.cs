namespace WebShop.Models.Interfaces
{
    public interface IProductManager
    {

        public Task<Product?> GetProduct(int id);
        public Task AddProduct(Product product);
        public Task UpdateProduct(Product oldProduct, Product newProduct);
        public Task DeleteProduct(int id);
    }
}