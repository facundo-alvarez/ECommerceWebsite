using ApplicationCore.Entities;

namespace ApplicationCore.ValueObjects
{
    public class Item
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
