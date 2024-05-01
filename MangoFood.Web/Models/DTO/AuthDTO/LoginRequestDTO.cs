using System.ComponentModel.DataAnnotations;

namespace MangoFood.Web.Models.DTO.AuthDTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
