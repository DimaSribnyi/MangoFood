namespace MangoFood.Web.Models.DTO.CartDTO
{
    public class ShoppingCartDTO
    {
        public ShoppingCartHeaderDTO CartHeader { get; set; }
        public IEnumerable<ShoppingCartDetailsDTO>? CartDetails { get; set; }
    }
}
