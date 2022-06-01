using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private ApplicationDbContext _db;

        public DiscountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public DiscountCode GetDiscoutCode(string code)
        {
            return _db.DiscountCodes.Where(d => d.Code == code).FirstOrDefault();
        }
    }
}
