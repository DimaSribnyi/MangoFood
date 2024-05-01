using Microsoft.AspNetCore.Identity;

namespace MangoFood.Services.AuthAPI.Models.Identites
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
