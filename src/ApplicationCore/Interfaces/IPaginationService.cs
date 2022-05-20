using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IPaginationService
    {
        List<Product> GetPaginatedResult(IEnumerable<Product> data, int currentPage, int pageSize);
    }
}
