using Yarp.Gateway.Extentions;

var builder = WebApplication.CreateBuilder(args);
builder.GetCORSConfiguration();
builder.GetAuthConfiguration();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.UseCors("AllowAllHeaders");

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.Run();
