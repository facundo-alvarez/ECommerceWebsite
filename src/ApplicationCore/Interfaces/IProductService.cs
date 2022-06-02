using ApplicationCore.Entities;
using ApplicationCore.Specifications;

namespace ApplicationCore.Interfaces
{
    public interface IProductService
    {
        IReadOnlyList<Product> GetProducts();
        IReadOnlyList<Product> GetProductsWithSpecification(Specification<Product> specification);
        IReadOnlyList<Product> GetRelatedProducts(Category category);
        Product GetProductById(int id);
        void AddProduct(Product product);
        void RemoveProduct(Product product);
        void UpdateProduct(Product product);
        void SaveProduct();
    }
}
