using FileManagement.Helper;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using Services;
using System.Security.Policy;

namespace FileManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExcelFileService _excelFIleService;
        private readonly string jwtToken;

        public ExcelController(IHttpContextAccessor httpContextAccessor, IExcelFileService excelFIleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _excelFIleService = excelFIleService;
            jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        }

        [HttpGet("ExportUsers")]
        public async Task<IActionResult> ExportUsers(bool isFrontEnd=true)
        {
            ExternalApiHelper externalApiHelper = new ExternalApiHelper(jwtToken);
            string responseString = await externalApiHelper.GetAsync(ApiEndPointEnum.GetAllUsers);
            var users = JsonConvert.DeserializeObject<List<UserDetailsModel>>(responseString);
            var data = users?.Select(x => new
            {
                x.UserId,
                x.UserName,
                x.Gender,
                DateOfBirth = x.BirthDate?.ToString("yyyy-MM-dd"),
                x.Email,
                x.MobileNo,
                x.Address.CountryName,
                x.Address.StateName,
                x.Address.CityName
            }).ToList();
            var fileStream = _excelFIleService.ExportToExcel(data);

            // Return the Excel file as the response
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Test.xlsx");
        }
        [HttpGet("ExportCities")]
        public async Task<IActionResult> ExportCities()
        {
            ExternalApiHelper externalApiHelper = new ExternalApiHelper(jwtToken);
            string responseString = await externalApiHelper.GetAsync(ApiEndPointEnum.GetAllCities); 
            var city = JsonConvert.DeserializeObject<List<City>>(responseString);
            
            var fileStream = _excelFIleService.ExportToExcel(city);

            // Return the Excel file as the response
            return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Test.xlsx");
        }
    }
}
