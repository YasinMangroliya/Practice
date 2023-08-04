using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AuthenticateResponse
    {
        public Int64 UserId { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }


        public AuthenticateResponse(Int64 userId, string userName, string? role, string? token, string? refreshToken)
        {
            UserId = userId;
            UserName = userName;
            Role= role;
            Token = token;
            RefreshToken = refreshToken;
        }
    }
    public class AuthRequest
    {
        public Int64 UserId { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

    }
    public class SSORequest
    {
        public string rToken { get; set; }
        public Int64 uId { get; set; }

    }
    public static class ClaimsEnum
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string RoleId = "RoleId";
    }

    public static class RoleEnum
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public const string User = "User";
        public const string Restricted = "Restricted";
    }
}
