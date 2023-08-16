using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.EFDataAccess;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManagementContext _userManagementContext;
        private readonly IConfiguration _configuration;
        public AuthenticationService(UserManagementContext userManagementContext, IConfiguration configuration)
        {
            _userManagementContext = userManagementContext;
            _configuration = configuration;
        }
        public AuthenticateResponse AuthenticateUser(UserDetailsModel userDetails)
        {

            if (userDetails == null)
                return null;
            var claims = GetClaims(userDetails.UserId.ToString(), userDetails.UserName, userDetails.RoleName);
            var token = generateJwtToken(claims);
            string refreshToken = GenerateRefreshToken();

            return new AuthenticateResponse(userDetails.UserId, userDetails.UserName, userDetails.RoleName, token, refreshToken);
        }

        public string generateJwtToken(ClaimsIdentity claims)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audiences"];
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:TokenExpireInMinutes"])),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string strToken = tokenHandler.WriteToken(token);

            return strToken;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string encodedString = Convert.ToBase64String(randomNumber);
            encodedString = encodedString.Replace(@"/", "z");
            return encodedString;
        }
        public async Task<Int64> UpdateTokenInDB(Int64 userId, string token, string refreshToken)
        {
            UserDetails user = new UserDetails();
            _userManagementContext.ChangeTracker.Clear();
            user.UserId = userId;
            user.RefreshToken = refreshToken;
            user.JwtToken = token;
            _userManagementContext.Entry(user).Property(x => x.RefreshToken).IsModified = true;
            _userManagementContext.Entry(user).Property(x => x.JwtToken).IsModified = true;
            return await _userManagementContext.SaveChangesAsync();
        }

        public ClaimsIdentity GetClaims(string UserId, string UserName, string RoleName)
        {
            return new ClaimsIdentity(new[] { new Claim(ClaimsEnum.UserId, UserId),
                                              new Claim(ClaimsEnum.UserName,UserName ),
                                              new Claim(ClaimTypes.Role,RoleName)
                                             });
        }
        public ClaimsPrincipal? GetPrincipalFromToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
