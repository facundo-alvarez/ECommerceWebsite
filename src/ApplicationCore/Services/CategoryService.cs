using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _genericRepositry;

        public CategoryService(IGenericRepository<Category> genericRepositry)
        {
            _genericRepositry = genericRepositry;
        }

        public IEnumerable<Category> GetCategories()
        {
            return _genericRepositry.GetAll();           
        }
    }
}
