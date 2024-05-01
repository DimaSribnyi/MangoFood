using MangoFood.Web.Models.DTO.OrderDTO;

namespace MangoFood.Web.Models.DTO
{
    public class StripeRequestDTO
    {
        public string SessionUrl { get; set; }
        public string SessionId { get; set; }
        public string CancelUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public OrderHeaderDTO OrderHeader { get; set; }
    }
}
