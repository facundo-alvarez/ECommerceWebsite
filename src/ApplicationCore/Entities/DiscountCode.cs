namespace ApplicationCore.Entities
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double PriceDiscount { get; set; }
    }
}
