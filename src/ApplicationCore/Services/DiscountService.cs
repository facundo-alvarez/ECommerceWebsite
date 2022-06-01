using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public decimal GetDiscount(string code, decimal total)
        {
            var discount = _discountRepository.GetDiscoutCode(code);

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
            return _discountRepository.GetDiscoutCode(code);
        }

        public string GetDiscountName(string code)
        {
            return _discountRepository.GetDiscoutCode(code).Name;
        }

        public bool IsValid(string code)
        {
            var discount = _discountRepository.GetDiscoutCode(code);

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
