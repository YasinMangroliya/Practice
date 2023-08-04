using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IAuthenticationService
    {
        AuthenticateResponse AuthenticateUser(UserDetailsModel userDetails);
        ClaimsPrincipal? GetPrincipalFromToken(string? token);
        string generateJwtToken(ClaimsIdentity claims);
        string GenerateRefreshToken();
        ClaimsIdentity? GetClaims(string UserId, string UserName, string RoleName);

        Task<Int64> UpdateTokenInDB(Int64 userId, string token, string refreshToken);
    }
}
