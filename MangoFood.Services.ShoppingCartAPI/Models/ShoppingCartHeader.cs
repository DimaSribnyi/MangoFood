using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFood.Services.ShoppingCartAPI.Models
{
    public class ShoppingCartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        [NotMapped]
        public double Discount { get; set; }

        [NotMapped]
        public double TotalAmount { get; set; }
    }
}
