using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories();
    }
}
