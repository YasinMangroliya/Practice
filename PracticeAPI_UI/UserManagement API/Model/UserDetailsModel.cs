using Microsoft.AspNetCore.Http;
using Model.Attribute;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public class UserDetailsModel
    {
        public UserDetailsModel()
        {
            Address = new AddressModel();
        }
        [Key]
        public Int64 UserId { get; set; }
        [Required, MaxLength(20), MinLength(4)]
        public string? UserName { get; set; }
        [Required, MaxLength(20), MinLength(4)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace")]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int MobileNo { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public DateTime? BirthDate { get; set; }
        public Int64 AddressId { get; set; }
        public string? ProfileImageName { get; set; }
        public string? ProfileImageBlob { get; set; }

        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.PNG|.JPG|.jpeg|.JPEG)$", ErrorMessage = "Only Image files allowed")]
        [MaxFileSizeInMb(2)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile? ProfileImageFile { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public Int64 CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;
        public Int64 ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RefreshToken { get; set; }
        public string? JwtToken { get; set; }
        public int LoginAttempt { get; set; }
        public DateTime? LoginAttemptDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public AddressModel Address { get; set; }

    }

    public class LoginModel
    {
        [Required, MaxLength(20), MinLength(4)]
        public string UserName { get; set; }
        [Required, MaxLength(20), MinLength(4)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password cannot contain whitespace")]
        public string Password { get; set; }
    }

    public static class LoginValidationEnum
    {
        public const string UserNotFound = "User not found";
        public const string InvalidPassword = "Invalid password";
        public const string InactiveUser = "User is not active";
        public const string MaximumAtempt = "Maximum login attempt limit reach, please try after some time";
    }


    public class UserDetailsList : PaginationParams
    {
        public UserDetailsList()
        {
            lstUserDetails = new List<UserDetailsModel>();
        }
        public List<UserDetailsModel> lstUserDetails { get; set; }
    }

    public static class UserDetailsEnum
    {
        public const string UserName = "username";
        public const string Email = "email";
    }
}