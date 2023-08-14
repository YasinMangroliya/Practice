using FileManagement.Helper;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Newtonsoft.Json;
using Services;

namespace FileManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPdfFileService _pdfFileService;
        private readonly string jwtToken;

        public PdfController(IHttpContextAccessor httpContextAccessor, IPdfFileService pdfFIleService)
        {
            _httpContextAccessor = httpContextAccessor;
            _pdfFileService = pdfFIleService;
            jwtToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        }
        [HttpGet("ExportUsers")]
        public async Task<IActionResult> ExportUsers()
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

            var dt = CommonService.ConvertListToDataTable(data);

            var fileStream =  _pdfFileService.ExportToPdf(dt);

            // Return the Excel file as the response
            return File(fileStream, "application/pdf", "Users.pdf");
        }
        [HttpGet("ExportCities")]
        public async Task<IActionResult> ExportCities()
        {
            ExternalApiHelper externalApiHelper = new ExternalApiHelper(jwtToken);
            string responseString = await externalApiHelper.GetAsync(ApiEndPointEnum.GetAllCities);
            var data = JsonConvert.DeserializeObject<List<City>>(responseString);

            var dt = CommonService.ConvertListToDataTable(data.Take(10000).ToList());

            var fileStream =  _pdfFileService.ExportToPdf(dt);

            // Return the Excel file as the response
            return File(fileStream, "application/pdf", "cities.pdf");
        }
    }
}
