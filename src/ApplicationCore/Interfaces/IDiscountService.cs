using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IDiscountService
    {
        bool IsValid(string code);
        decimal GetDiscount(string code, decimal total);
        DiscountCode GetDiscountByCode(string code);
        string GetDiscountName(string code);
    }
}
