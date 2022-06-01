namespace ApplicationCore.Entities
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal PriceDiscount { get; set; }
        public string Type { get; set; }
    }
}
