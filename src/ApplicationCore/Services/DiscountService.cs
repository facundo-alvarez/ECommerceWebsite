using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IGenericRepository<DiscountCode> _genericRepository;

        public DiscountService(IGenericRepository<DiscountCode> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public decimal GetDiscount(string code, decimal total)
        {
            var discount = _genericRepository.GetAll().Where(d => d.Code == code).FirstOrDefault();

                if (discount.Type == "percentage")
                {
                    total = total - (total * discount.PriceDiscount / 100);
                    return total;
                }
                else
                {
                    total -= discount.PriceDiscount;
                    if (total < 0)
                        return 0;
                    return total;
                }
        }

        public DiscountCode GetDiscountByCode(string code)
        {
            return _genericRepository.GetAll().Where(d => d.Code == code).FirstOrDefault();
        }

        public bool IsValid(string code)
        {
            var discount = _genericRepository.GetAll().Where(d => d.Code == code).FirstOrDefault();

            if (discount != null)
            {
                if(discount.CreatedDate > DateTime.Today || discount.ValidUntil < DateTime.Today)
                    return false;

                return true;
            }
            else
                return false;
        }
    }
}
