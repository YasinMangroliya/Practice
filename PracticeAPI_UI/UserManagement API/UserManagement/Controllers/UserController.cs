using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.EFDataAccess;
using Services;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAllUsers")]
        [Authorize(Roles =("Admin,Customer"))]
        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _userService.GetAllUsers();
            return new OkObjectResult(userList);
        }

        [HttpGet("GetUserList")]
        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> GetUserList([FromQuery] DataTableParams dataTableParams, DateTime? fromDate = null, DateTime? toDate = null, string? locationIds = null, string? locationBy = null)
        {
            var userList = await _userService.GetUserList(dataTableParams, fromDate, toDate, locationIds, locationBy);
            return new OkObjectResult(userList);
        }
        [HttpPost("SaveUserDetails")]
        public async Task<IActionResult> SaveUserDetails([FromForm] UserDetailsModel userDetailsModel)
        {
            Int64 userId = await _userService.SaveUserDetails(userDetailsModel);
            return new OkObjectResult(userId);
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<IActionResult> GetUserById(Int64 userId)
        {
            var userDetail = await _userService.GetUserById(userId);
            return new OkObjectResult(userDetail);
        }
        [HttpGet("CheckUserNameIsUnique/{userId}/{userName}")]
        public async Task<IActionResult> CheckUserNameIsUnique(Int64 userId, string userName)
        {
            bool isUserExist = await _userService.CheckUserNameExist(userId, userName);
            return Ok(isUserExist);
        }

        [HttpGet("DeleteUserById/{userId}")]
        public async Task<IActionResult> DeleteUserById(Int64 userId)
        {
            bool response = await _userService.DeleteUserById(userId);
            return new OkObjectResult(response);
        }
        //for DropDown...
        [HttpGet("CountryList")]
        public async Task<IActionResult> CountryList()
        {
            var countryList = await _userService.GetCountryList();
            return new OkObjectResult(countryList);

        }
        [HttpGet("StateByCountry/{countryId}")]
        public async Task<IActionResult> StateByCountry(Int64 countryId)
        {
            var stateList = await _userService.GetStateListByCountryId(countryId);
            return new OkObjectResult(stateList);
        }
        [HttpGet("CityByState/{stateId}")]
        public async Task<IActionResult> CityByState(Int64 stateId)
        {
            var cityList = await _userService.GetCityListByStateId(stateId);
            return new OkObjectResult(cityList);

        }
        [HttpGet]
        [Route("GetAllCities")]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _userService.GetAllCities();
            return new OkObjectResult(cities);
        }

    }
}
