using ApplicationCore.ValueObjects;

namespace ApplicationCore.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public IEnumerable<Order_Product> Order_Product { get; set; } = new List<Order_Product>();

        public bool HasCupon { get; set; } = false;

        public int? DiscountCodeId { get; set; }
        public virtual DiscountCode DiscountCode { get; set; }

        public decimal SubTotal { get; set; } = 0;

        public decimal Total { get; set; } = 0;

        public string OrderStatus { get; set; } = string.Empty;

        public int? AddressId { get; set; }
        public Address DeliveryAddress { get; set; }
    }
}
