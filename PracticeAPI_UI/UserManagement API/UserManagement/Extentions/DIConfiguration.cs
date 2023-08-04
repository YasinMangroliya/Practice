using Infrastructure;
using Services;

namespace UserManagement.Extentions
{
    public static class DIConfiguration
    {
        public static void GetDIConfiguration(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IUserService,UserService>();
            webApplicationBuilder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}
