using ApplicationCore.Entities;

namespace ApplicationCore.ValueObjects
{
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal Total
        {
            get 
            { 
                return Product.Price * Quantity; 
            }
        }
    }
}
