using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Model.EFDataAccess;

namespace UserManagement.Extentions
{
    public static class DBConfiguration
    {
        public static void GetDBConfiguration(this WebApplicationBuilder webApplicationBuilder ,string connectionString)
        {
            webApplicationBuilder.Services.AddDbContext<UserManagementContext>(options => options
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Scoped);

        }
    }
}
