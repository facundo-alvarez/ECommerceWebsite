using ApplicationCore.Entities;
using ApplicationCore.Interfaces;


namespace ApplicationCore.Services
{
    public class PaginationService : IPaginationService
    {
        public List<Product> GetPaginatedResult(IEnumerable<Product> data, int currentPage, int pageSize)
        {
            var result = data.OrderBy(d => d.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }
    }
}

