using FileManagement.Extentions;
using FileManagement.Helper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


//builder.GetCORSConfiguration();


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.GetSwaggerConfiguration();
builder.GetDIConfiguration();
ApiEndPoint.WebHostEnvironment = builder.Environment.EnvironmentName;
string connectionString = builder.Configuration.GetConnectionString(builder.Environment.EnvironmentName);

var logger = builder.SerilogConfig(connectionString);

builder.Host.UseSerilog();
//builder.GetAuthConfiguration();
builder.GetModelStateLogger(logger);


var app = builder.Build();

//app.UseCors("AllowAllHeaders");
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

app.MapControllers();

app.Run();
