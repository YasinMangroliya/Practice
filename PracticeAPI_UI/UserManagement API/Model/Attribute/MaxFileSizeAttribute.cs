using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Attribute
{
    public class MaxFileSizeInMb : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeInMb(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (convertInMB(file.Length) > convertInMB(_maxFileSize))
                {
                    return new ValidationResult(GetErrorMessage(file.FileName));
                }
            }

            //For Multiple Files
            //var files = value as IList<IFormFile>;
            //foreach (var file in files)
            //{
            //    if (file != null)
            //    {
            //        if (file.Length > _maxFileSize)
            //        {
            //            return new ValidationResult(GetErrorMessage(file.FileName));
            //        }
            //    }
            //}

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string name)
        {
            return $"{name}'s size is out of range. Maximum allowed file size is {_maxFileSize} MB.";
        }
        public long convertInMB(long size)
        {
            long sizeInKB = 1024 * 1024;
            return size / sizeInKB;
        }
    }
}
