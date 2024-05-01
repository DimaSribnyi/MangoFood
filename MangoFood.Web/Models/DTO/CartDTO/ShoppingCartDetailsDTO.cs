namespace MangoFood.Web.Models.DTO.CartDTO
{
    public class ShoppingCartDetailsDTO
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public ShoppingCartHeaderDTO? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
    }
}
