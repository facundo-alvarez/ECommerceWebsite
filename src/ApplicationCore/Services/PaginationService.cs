using ApplicationCore.Entities;
using ApplicationCore.Interfaces;


namespace ApplicationCore.Services
{
    public class PaginationService : IPaginationService
    {
        private readonly IProductService _productService;        

        public PaginationService(IProductService productService)
        {
            _productService = productService;
        }

        public List<Product> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            var data = _productService.GetProducts();
            return data.OrderBy(d => d.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetCount()
        {
            var data = _productService.GetProducts();
            return data.Count(); 
        }
    }
}

