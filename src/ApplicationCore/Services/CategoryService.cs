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


        public IReadOnlyList<Category> GetCategories()
        {
            return _genericRepositry.GetAll().ToList();
        }

        public Category GetCategory(string name)
        {
            return _genericRepositry.GetAll().Where(c => c.Name.ToLower() == name).FirstOrDefault();
        }
    }
}
