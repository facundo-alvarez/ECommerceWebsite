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

        public bool IsValid(string code)
        {
            var discount = _discountRepository.GetDiscoutCode(code);

            if (discount == null)
            {
                return false;
            }
            else
                return true;
        }
    }
}
