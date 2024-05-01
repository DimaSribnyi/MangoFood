using MangoFood.Services.ShoppingCartAPI.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFood.Services.ShoppingCartAPI.Models
{
    public class ShoppingCartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }

        [ForeignKey(nameof(CartHeaderId))]
        public ShoppingCartHeader CartHeader { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
    }
}
