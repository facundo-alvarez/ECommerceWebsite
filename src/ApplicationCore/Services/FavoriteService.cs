using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IGenericRepository<User_Product> _genericRepository;

        public FavoriteService(IGenericRepository<User_Product> genericRepository)
        {
            _genericRepository = genericRepository;
        }


        public void AddToFavorite(User_Product userProduct)
        {
            _genericRepository.Insert(userProduct);
            _genericRepository.Save();
        }

        public void RemoveFromFavorite(int id)
        {
            _genericRepository.Delete(id);
            _genericRepository.Save();
        }

        public IEnumerable<User_Product> GetUserProducts(string userId)
        {
            return _genericRepository.GetAll().Where(u => u.UserId == userId);
        }

        public int GetId(User_Product userProduct)
        {
            var id = _genericRepository.GetAll().Where(up => up.ProductId == userProduct.ProductId && up.UserId == userProduct.UserId).Select(i => i.Id).FirstOrDefault();
            return id;
        }
    }
}
