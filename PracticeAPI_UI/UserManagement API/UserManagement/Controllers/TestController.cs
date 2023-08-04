using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.EFDataAccess;
using Services;
using static System.Net.Mime.MediaTypeNames;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUserService _userService;

        public TestController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            string test = "Response From Get";
            return new OkObjectResult(new { test });
        }
        [HttpPost("PostUser")]
        public IActionResult PostUser(LoginModel loginModel)
        {
            string test = "LoggedIn Success with UserName" + loginModel.UserName;

            return new OkObjectResult(test);
        }
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id)
        {
            string test = "Put Response with id " + id.ToString();
            return new OkObjectResult(new { test });
        }
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            string test = "Delete Response with id " + id.ToString();
            return new OkObjectResult(new { test });
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var user = await _userService.LoginUser(loginModel.UserName);
            //_userService.ConvertDateTimeToMinute(user?.CreatedDate);
            if (user == null)
            {
                return BadRequest(LoginValidationEnum.UserNotFound);
            }
            else if (user.LoginAttempt >= 3 && _userService.ConvertDateTimeToMinute(user.LoginAttemptDateTime) < 5)
            {
                //_userService.UpdateOrClearLoginAttempt(user);
                return BadRequest(LoginValidationEnum.MaximumAtempt); ;
            }
            else if (user.Password != loginModel.Password)
            {
                _userService.UpdateOrClearLoginAttempt(user);
                return BadRequest(LoginValidationEnum.InvalidPassword);
            }
            else if (!user.IsActive)
            {
                _userService.UpdateOrClearLoginAttempt(user);
                return BadRequest(LoginValidationEnum.InactiveUser);
            }

            return new OkObjectResult(true);
        }

    }
}
