namespace MangoFood.Web.Models.DTO.AuthDTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
