using System.ComponentModel.DataAnnotations;

namespace MangoFood.Web.Utility
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSiex;

        public MaxFileSizeAttribute(int maxFileSiex)
        {
            _maxFileSiex = maxFileSiex;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if(file.Length > (_maxFileSiex * 1024 * 1024))
                {
                    return new ValidationResult($"File is too big, maximus size is {_maxFileSiex} MB");
                }
            }

            return ValidationResult.Success;
        }
    }
}
