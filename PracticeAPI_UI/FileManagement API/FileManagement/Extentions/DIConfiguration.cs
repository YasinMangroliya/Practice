using Infrastructure;
using Services;

namespace FileManagement.Extentions
{
    public static class DIConfiguration
    {
        public static void GetDIConfiguration(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IExcelFileService, ExcelFileService>();
            webApplicationBuilder.Services.AddScoped<IPdfFileService, PdfFileService>();
        }
    }
}
