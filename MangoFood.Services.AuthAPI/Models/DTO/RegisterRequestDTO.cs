namespace MangoFood.Services.AuthAPI.Models.DTO
{
    public class RegisterRequestDTO
    {
        public string? ID { get; set; }

        //[Required]
        public string Name { get; set; }

        //Required]
        //[EmailAddress]
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required]
        //[Phone]
        //[DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
