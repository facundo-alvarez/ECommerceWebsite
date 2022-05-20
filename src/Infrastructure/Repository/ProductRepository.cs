using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetProducts()
        {
            return _db.Products;
        }

        public Product GetProductById(int id)
        {
            return _db.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product GetProductByIdNoTracking(int id)
        {
            return _db.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public void AddProduct(Product product)
        {
            _db.Products.Add(product);
        }

        public void SaveProduct()
        {
            _db.SaveChanges();
        }

        public void RemoveProduct(Product product)
        {
            _db.Products.Remove(product);
        }

        public void UpdateProduct(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
