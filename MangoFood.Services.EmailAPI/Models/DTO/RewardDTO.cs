namespace MangoFood.Services.EmailAPI.Models.DTO
{
    public class RewardDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int RewardActivity { get; set; }
        public int OrderId { get; set; }
    }
}
