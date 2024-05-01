namespace MangoFood.Services.EmailAPI.Models.DTO
{
    public class ShoppingCartDTO
    {
        public ShoppingCartHeaderDTO CartHeader { get; set; }
        public IEnumerable<ShoppingCartDetailsDTO>? CartDetails { get; set; }
    }
}
