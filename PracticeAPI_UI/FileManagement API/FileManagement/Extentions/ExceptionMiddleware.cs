using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.AspNetCore.WebUtilities;
using Model;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core.Enrichers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FileManagement.Extentions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, Serilog.ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    ErrorDetailsModel errorDetailsModel = new ErrorDetailsModel();
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    errorDetailsModel.ExceptionType = contextFeature?.Error.GetType().ToString();
                    errorDetailsModel.StatusCode = context.Response.StatusCode;
                    errorDetailsModel.StatusMessage = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode);
                    errorDetailsModel.Path = context.Request.Host + context.Request.Path;
                    errorDetailsModel.EndPoint = contextFeature?.Endpoint?.ToString();

                    errorDetailsModel.Message = contextFeature?.Error.Message;
                    errorDetailsModel.Exception = contextFeature.Error;
                    if (contextFeature != null)
                    {
                        logger.ForContext(ErrorDetailsEnum.ExceptionType, errorDetailsModel.ExceptionType)
                              .ForContext(ErrorDetailsEnum.StatusCode, errorDetailsModel.StatusCode)
                              .ForContext(ErrorDetailsEnum.StatusMessage, errorDetailsModel.StatusMessage)
                              .ForContext(ErrorDetailsEnum.Path, errorDetailsModel.Path)
                              .ForContext(ErrorDetailsEnum.EndPoint, errorDetailsModel.EndPoint)
                              .ForContext(ErrorDetailsEnum.UserId, 123)
                              .Error(errorDetailsModel.Exception, errorDetailsModel.Message);

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetailsModel));
                    }
                });
            });
        }
    }
}
