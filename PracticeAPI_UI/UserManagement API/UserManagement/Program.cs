using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Model;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Net;
using System.Text;
using UserManagement.Extentions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.GetCORSConfiguration();
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddResponseCaching();
    builder.GetSwaggerConfiguration();


    string connectionString = builder.Configuration.GetConnectionString(builder.Environment.EnvironmentName);
    builder.GetDBConfiguration(connectionString);

    builder.GetDIConfiguration();

    var logger = builder.SerilogConfig(connectionString);
    
    builder.Host.UseSerilog();
    builder.GetAuthConfiguration();
    builder.GetModelStateLogger(logger);


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseCors("AllowAllHeaders");
    app.UseSerilogRequestLogging();
    app.ConfigureExceptionHandler(logger);
    app.UseResponseCaching();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();

    //app.UseMiddleware<JwtMiddleware>();
    app.MapControllers();
    app.Run();

}
catch (Exception ex)
{
    throw;
}


