using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IPaginationService
    {
        List<Product> GetPaginatedResult(IEnumerable<Product> data, int currentPage, int pageSize);
    }
}
