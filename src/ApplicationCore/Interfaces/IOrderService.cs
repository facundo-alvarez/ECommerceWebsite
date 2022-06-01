using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        Order GetUserCurrentOrder(string id);
        void SaveOrder(Order order);
        void UpdateOrder(Order order);
        void InsertOrder(Order order);
        void DeleteOrder(Order order);
    }
}
