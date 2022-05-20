using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        void AddProduct(Product product);
        void SaveProduct();
        void RemoveProduct(Product product);
        void UpdateProduct(Product product);
        Product GetProductByIdNoTracking(int id);
    }
}
