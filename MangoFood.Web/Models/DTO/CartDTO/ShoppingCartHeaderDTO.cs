using System.ComponentModel.DataAnnotations;

namespace MangoFood.Web.Models.DTO.CartDTO
{
    public class ShoppingCartHeaderDTO
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }

        [Required]
        public string? FirstName {  get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Phone { get; set; }
    }
}
