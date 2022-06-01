using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ValueObjects
{
    public class Item
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
