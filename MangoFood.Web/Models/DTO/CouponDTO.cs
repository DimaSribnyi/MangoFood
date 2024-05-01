namespace MangoFood.Web.Models.DTO
{
    public class CouponDTO
    {
        public Guid CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}
