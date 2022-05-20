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
        List<Product> GetPaginatedResult(int currentPage, int pageSize = 10);
        int GetCount();
    }
}
