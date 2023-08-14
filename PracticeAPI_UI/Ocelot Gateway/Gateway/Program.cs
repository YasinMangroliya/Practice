using Gateway.Extentions;
using Microsoft.Extensions.Configuration;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.GetCORSConfiguration();
builder.GetAuthConfiguration();
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddCacheManager(x =>
{
    x.WithDictionaryHandle();
}); 

var app = builder.Build();
app.UseCors("AllowAllHeaders");

app.UseAuthentication();
app.UseAuthorization();
app.UseOcelot().GetAwaiter().GetResult();
app.MapGet("/", () => "Hello World!");
app.Run();
