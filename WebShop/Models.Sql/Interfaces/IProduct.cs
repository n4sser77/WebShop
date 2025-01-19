namespace WebShop.Models.Interfaces
{
    internal interface IProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}