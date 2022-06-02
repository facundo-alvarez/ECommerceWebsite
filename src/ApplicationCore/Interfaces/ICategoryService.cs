using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        IReadOnlyList<Category> GetCategories();
        Category GetCategory(string name);
    }
}
