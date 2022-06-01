using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IDiscountRepository
    {
        DiscountCode GetDiscoutCode(string code);
    }
}
