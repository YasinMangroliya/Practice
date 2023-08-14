namespace Gateway.Extentions
{
    public static class CORSConfiguration
    {
        public static void GetCORSConfiguration(this WebApplicationBuilder webApplicationBuilder)
        {
            IConfigurationSection configurationSection = webApplicationBuilder.Configuration.GetSection("Cors:AllowedOrigins");
            var hosts = configurationSection.Get<List<string>>();
            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders", builder => builder
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .WithOrigins(hosts.ToArray())
                 .AllowCredentials());
            });
        }
    }
}
