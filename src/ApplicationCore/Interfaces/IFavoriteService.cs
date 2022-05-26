using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IFavoriteService
    {
        void AddToFavorite(User_Product userProduct);
        void RemoveFromFavorite(int id);
        IEnumerable<User_Product> GetUserProducts(string userId);
        int GetId(User_Product userProduct);
    }
}
