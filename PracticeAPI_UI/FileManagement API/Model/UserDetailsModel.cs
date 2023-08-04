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
        public Int64 UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        
        public string? Email { get; set; }
        
        public int MobileNo { get; set; }
        
        public string? Gender { get; set; }
        
        public DateTime? BirthDate { get; set; }
        public Int64 AddressId { get; set; }
        public string? ProfileImageName { get; set; }
        public string? ProfileImageBlob { get; set; }

        //public IFormFile? ProfileImageFile { get; set; }
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

    public static class LoginValidationEnum
    {
        public const string UserNotFound = "User not found";
        public const string InvalidPassword = "Invalid password";
        public const string InactiveUser = "User is not active";
        public const string MaximumAtempt = "Maximum login attempt limit reach, please try after some time";
    }

    public static class UserDetailsEnum
    {
        public const string UserName = "username";
        public const string Email = "email";
    }
}