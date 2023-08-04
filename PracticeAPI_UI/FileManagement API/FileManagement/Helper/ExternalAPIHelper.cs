using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileManagement.Helper
{
    public class ExternalApiHelper
    {
        private readonly HttpClient _httpClient;

        public ExternalApiHelper(string jwtToken)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        }

        public async Task<string> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> PostAsync(string url, string content)
        {
            var httpContent = new StringContent(content);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }





    public static class ApiEndPoint
    {
        public static string WebHostEnvironment { get; set; }


        public static string _userManagementPI = string.Empty;
        static ApiEndPoint()
        {
            var configurationBuilder = new ConfigurationBuilder();

            string _host = null;
            if (!string.IsNullOrEmpty(WebHostEnvironment))
            {
                _host = "appsettings." + WebHostEnvironment + ".json";
            }
            else
            {
                _host = "appsettings.json";
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), _host);
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _userManagementPI = root.GetSection("CrossApiUrl").GetSection("UserManagement").Value;
        }
    }
    public static class ApiEndPointEnum
    {
        private static string UserManagementAPI = ApiEndPoint._userManagementPI;


        public static string GetAllUsers = UserManagementAPI + "User/GetAllUsers";
        public static string GetAllCities = UserManagementAPI + "User/GetAllCities";

    }
}
