using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _genericRepository;

        public ProductService(IGenericRepository<Product> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public IReadOnlyList<Product> GetProducts()
        {
            return _genericRepository.GetAll().ToList();
        }

        public Product GetProductById(int id)
        {
            return _genericRepository.Get(id);
        }

        public void AddProduct(Product product)
        {
            _genericRepository.Insert(product);
        }

        public void RemoveProduct(Product product)
        {
            _genericRepository.Delete(product);
        }

        public void UpdateProduct(Product product)
        {
            _genericRepository.Update(product);
        }

        public void SaveProduct()
        {
            _genericRepository.Save();
        }

        public IReadOnlyList<Product> GetRelatedProducts(Category category)
        {
            return _genericRepository.GetAll().Where(p => p.CategoryId == category.Id).ToList();
        }

        public IReadOnlyList<Product> GetProductsWithSpecification(Specification<Product> specification)
        {
            return _genericRepository.GetAll().Where(specification.ToExpression()).ToList();
        }
    }
}

