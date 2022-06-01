using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.ValueObjects;

namespace ApplicationCore.Services
{
    public class OrderProductService : IOrderProductService
    {
        private readonly IGenericRepository<Order_Product> _genericRepository;
        private readonly IProductService _productService;

        public OrderProductService(IGenericRepository<Order_Product> genericRepository, IProductService productService)
        {
            _genericRepository = genericRepository;
            _productService = productService;
        }

        public IEnumerable<Order_Product> GetOrderCurrentProducts(int orderId)
        {
            return _genericRepository.GetAll().Where(o => o.OrderId == orderId);
        }

        public decimal GetOrderSubtotal(int orderId)
        {
            decimal subtotal = 0;
            var products = _genericRepository.GetAll().Where(o => o.OrderId == orderId);
            
            foreach (var product in products)
            {           
                subtotal += _productService.GetProductById(product.ProductId).Price * product.Quantity;
            }
            _genericRepository.Save();
            return subtotal;
        }

        public void RemoveProductFormCurrentOrder(int orderId, int productId)
        {
            var orderProductId = _genericRepository.GetAll().Where(o => o.OrderId == orderId && o.ProductId == productId).Select(o => o.Id).FirstOrDefault();
            _genericRepository.Delete(orderProductId);
            _genericRepository.Save();
        }

        public void SetOrderProducts(List<Item> items, int orderId)
        {
            var allProducts = GetOrderCurrentProducts(orderId);

            foreach (var product in items)
            {
                if(allProducts.Any(p => p.ProductId == product.ProductId))
                {
                    AddProductQuantity(orderId, product.ProductId, product.Quantity);
                }
                else
                {
                    _genericRepository.Insert(new Order_Product()
                    {
                        ProductId = product.ProductId,
                        OrderId = orderId,
                        Quantity = product.Quantity
                    });

                }
            }
            _genericRepository.Save();
        }

        public void AddProductQuantity(int orderId, int productId, int quantity)
        {
            var product = _genericRepository.GetAll().Where(o => o.OrderId == orderId && o.ProductId == productId).FirstOrDefault();

            if (product.Quantity < 100)
            {
                product.Quantity += quantity;
                _genericRepository.Update(product);
                _genericRepository.Save();
            }
        }

        public void RemoveProductQuantity(int orderId, int productId, int quantity)
        {
            var product = _genericRepository.GetAll().Where(o => o.OrderId == orderId && o.ProductId == productId).FirstOrDefault();

            if (product.Quantity > 1)
            {
                product.Quantity -= quantity;
                _genericRepository.Update(product);
                _genericRepository.Save();
            }
        }
    }
}
