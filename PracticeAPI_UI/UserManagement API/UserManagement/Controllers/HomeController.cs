using Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.EFDataAccess;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class HomeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public HomeController(IUserService userService, IAuthenticationService authenticationService, IConfiguration configuration, UserManagementContext userManagementContext)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }


        [HttpGet("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser([FromQuery] LoginModel loginModel)
        {
            var user = await _userService.LoginUser(loginModel.UserName);
            Int64 result = 0;  //Void method not await to finished async

            if (user == null)
            {
                return BadRequest(new { message = LoginValidationEnum.UserNotFound });
            }
            else if (user.LoginAttempt >= 3 && _userService.ConvertDateTimeToMinute(user.LoginAttemptDateTime) < 5)
            {
                return BadRequest(new { message = LoginValidationEnum.MaximumAtempt });
            }
            else if (user.Password != loginModel.Password)
            {
                result = await _userService.UpdateOrClearLoginAttempt(user);
                return BadRequest(new { message = LoginValidationEnum.InvalidPassword });
            }
            else if (!user.IsActive)
            {
                result = await _userService.UpdateOrClearLoginAttempt(user);
                return BadRequest(new { message = LoginValidationEnum.InactiveUser });
            }
            result = await _userService.UpdateOrClearLoginAttempt(user, true);

            var response = _authenticationService.AuthenticateUser(user);
            Int64 res = await _authenticationService.UpdateTokenInDB(response.UserId, response.Token, response.RefreshToken);

            return new OkObjectResult(response);
        }
        [HttpGet]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] AuthRequest authRequest)
        {
            if (authRequest is null) return Unauthorized("Invalid client request");

            var principal = _authenticationService.GetPrincipalFromToken(authRequest.Token);

            if (principal == null) return Unauthorized("Invalid access token or refresh token");

            var userId = principal.Claims.First(x => x.Type == ClaimsEnum.UserId).Value.ToString();
            var userName = principal.Claims.First(x => x.Type == ClaimsEnum.UserName).Value.ToString();
            var role = principal.Claims.First(x => x.Type == ClaimTypes.Role).Value.ToString();

            var claims = _authenticationService.GetClaims(userId, userName, role);
            if (userId is not null)
            {
                var user = await _userService.GetUserById(Convert.ToInt64(userId));
                if (user == null || user.RefreshToken != authRequest.RefreshToken) return Unauthorized("Invalid access token or refresh token");
            }

            authRequest.Token = _authenticationService.generateJwtToken(claims);
            Int64 res = await _authenticationService.UpdateTokenInDB(authRequest.UserId, authRequest.Token, authRequest.RefreshToken);

            return new ObjectResult(authRequest);
        }


        [HttpPost]
        [Route("SSOAuthentication")]
        public async Task<IActionResult> SSOAuthentication(SSORequest sSORequest)
        {
            if (sSORequest.rToken is null || sSORequest.uId == 0) return Unauthorized("Invalid client request");

            var user = await _userService.GetUserById(Convert.ToInt64(sSORequest.uId));

            if (user == null || user.RefreshToken != sSORequest.rToken) return Unauthorized("Invalid access token or refresh token");

            var principal = _authenticationService.GetPrincipalFromToken(user.JwtToken);

            if (principal == null) return Unauthorized("Invalid access token or refresh token");

            //var userId = principal.Claims.First(x => x.Type == ClaimsEnum.UserId).Value.ToString();
            //var userName = principal.Claims.First(x => x.Type == ClaimsEnum.UserName).Value.ToString();
            //var role = principal.Claims.First(x => x.Type == ClaimTypes.Role).Value.ToString();

            return new ObjectResult(new AuthenticateResponse(user.UserId, user.UserName, user.RoleName, user.JwtToken, sSORequest.rToken));
        }
        [HttpGet]
        [Route("Logout/{userId}")]
        public IActionResult Logout(Int64 userId)
        {
            var id = _userService.LogOutUser(userId);

            return new OkObjectResult(id);
        }

    }
}
