using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Model;
using System.Net;

namespace FileManagement.Extentions
{
    public static class ModelStateLogger
    {
        public static void GetModelStateLogger(this WebApplicationBuilder builder, Serilog.ILogger logger)
        {
            builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //var modelState = actionContext.ModelState.Values;
                    var errorMessages = actionContext.ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                   .ToList();

                    // Convert the error messages to a single string
                    var errorMessageString = string.Join("; ", errorMessages);
                    ErrorDetailsModel errorDetailsModel = new ErrorDetailsModel();
                    errorDetailsModel.ExceptionType = "Invalid Model State";
                    errorDetailsModel.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetailsModel.StatusMessage = HttpStatusCode.BadRequest.ToString();
                    errorDetailsModel.Path = actionContext.HttpContext.Request.Host + actionContext.HttpContext.Request.Path;
                    errorDetailsModel.EndPoint = actionContext.HttpContext.GetEndpoint().ToString();

                    errorDetailsModel.Message = errorMessageString;
                    errorDetailsModel.Exception = null;

                    logger.ForContext(ErrorDetailsEnum.ExceptionType, errorDetailsModel.ExceptionType)
                          .ForContext(ErrorDetailsEnum.StatusCode, errorDetailsModel.StatusCode)
                          .ForContext(ErrorDetailsEnum.StatusMessage, errorDetailsModel.StatusMessage)
                          .ForContext(ErrorDetailsEnum.Path, errorDetailsModel.Path)
                          .ForContext(ErrorDetailsEnum.EndPoint, errorDetailsModel.EndPoint)
                          .ForContext(ErrorDetailsEnum.UserId, 123)
                          .Error(errorDetailsModel.Exception, errorDetailsModel.Message);
                    return new BadRequestObjectResult(actionContext.ModelState.Values);
                };
            });

        }
    }
}
