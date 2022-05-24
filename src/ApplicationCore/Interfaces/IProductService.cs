using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        Product GetProductByIdNoTracking(int id);
        IEnumerable<Product> GetRelatedProducts(Category category);
        void AddProduct(Product product);
        void SaveProduct();
        void RemoveProduct(Product product);
        void UpdateProduct(Product product);
    }
}
