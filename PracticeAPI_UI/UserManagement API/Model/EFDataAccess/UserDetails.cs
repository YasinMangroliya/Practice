using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.EFDataAccess
{
    public class UserDetails
    {
        public UserDetails()
        {
            Address = new Address();
        }
        [Key]
        public Int64 UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int MobileNo { get; set; }
        public char? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Int64 AddressId { get; set; }
        public string? ProfileImageName { get; set; }
        public string? ProfileImageBlob { get; set; }
        public DateTime? CreatedDate { get; set; } 
        public Int64 CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Int64 ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public string? JwtToken { get; set; }
        public int LoginAttempt { get; set; }
        public DateTime? LoginAttemptDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public Address Address { get; set; }

    }
    public class M_UserRole
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

    }
}