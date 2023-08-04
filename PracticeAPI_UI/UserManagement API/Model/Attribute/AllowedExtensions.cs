using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Attribute
{
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensions(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            var extension = Path.GetExtension(file?.FileName);
            if (file != null)
            {
                if (!_extensions.Contains(extension?.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage(extension));
                }
            }
            //var files = value as IList<IFormFile>;
            //foreach (var file in files)
            //{
            //    var extension = Path.GetExtension(file.FileName);
            //    if (file != null)
            //    {
            //        if (!_extensions.Contains(extension.ToLower()))
            //        {
            //            return new ValidationResult(GetErrorMessage(file.FileName));
            //        }
            //    }
            //}

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string name)
        {
            return $"{name} extension is not allowed!";
        }
    }
}
