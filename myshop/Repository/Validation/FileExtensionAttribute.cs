using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace myshop.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { ".jpg", ".png", ".jpeg" };
                bool result = extensions.Any(x => extension.EndsWith(x, StringComparison.OrdinalIgnoreCase));
                if (!result)
                {
                    return new ValidationResult("Allowed");
                }
            }

            return ValidationResult.Success;
        }
    }
}
