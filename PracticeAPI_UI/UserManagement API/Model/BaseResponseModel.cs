using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Model
{
    public class BaseResponseModel<T>
    {
        public Int64 Id { get; set; }
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }

        public T? ResponseContent { get; set; }
    }
    public class ErrorDetailsModel
    {
        public Exception Exception { get; set; }
        public string? ExceptionType { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? EndPoint { get; set; }
        public string? Path { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public static class ErrorDetailsEnum
    {
        public const string Exception = "Exception";
        public const string ExceptionType = "ExceptionType";
        public const string Message = "Message";
        public const string StatusCode = "StatusCode";
        public const string StatusMessage = "StatusMessage";
        public const string EndPoint = "EndPoint";
        public const string Path = "Path";
        public const string UserId = "UserId";
    }
}
