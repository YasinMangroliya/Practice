using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Yarp.Gateway.Extentions
{
    public static class AuthConfiguration
    {
        public static void GetAuthConfiguration(this WebApplicationBuilder builder)
        {
            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = builder.Configuration["JWT:Audiences"],
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                         ClockSkew = TimeSpan.Zero //Expired token throw unauthrize quickly
                };
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOrCustomerPolicy", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "Admin", "Customer");
                });
            });
        }
    }
}
