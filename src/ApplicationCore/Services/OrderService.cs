using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _genericRepository;

        public OrderService(IGenericRepository<Order> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public Order GetUserCurrentOrder(string id)
        {
            return _genericRepository.GetAll().Where(o => o.UserId == id && o.OrderStatus == "In process").FirstOrDefault();
        }

        public void SaveOrder(Order order)
        {
            _genericRepository.Save();
        }

        public void UpdateOrder(Order order)
        {
            _genericRepository.Update(order);
            _genericRepository.Save();
        }

        public void InsertOrder(Order order)
        {
            _genericRepository.Insert(order);
        }

        public void DeleteOrder(Order order)
        {
            _genericRepository.Delete(order.Id);
            _genericRepository.Save();
        }

        public IReadOnlyList<Order> GetAllUserOrders(string id)
        {
            return _genericRepository.GetAll().Where(o => o.UserId == id).ToList();
        }

        public IReadOnlyList<Order> GetAllUserOrdersWithYear(string year, string id)
        {
            return _genericRepository.GetAll().Where(o => o.OrderDate.Value.Year.ToString() == year && o.UserId == id).ToList();
        }
    }
}
