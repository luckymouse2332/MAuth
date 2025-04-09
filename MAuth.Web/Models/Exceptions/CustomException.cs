using MAuth.Web.Models.Responses;
using System.Diagnostics.CodeAnalysis;

namespace MAuth.Web.Models.Exceptions
{
    /// <summary>
    /// 自定义异常类
    /// </summary>
    public class CustomException(int code, string message) : Exception(message)
    {
        public int StatusCode { get; set; } = code;

        public override string ToString()
        {
#if DEBUG
            return base.ToString();
#else
            return Message;
#endif
        }

        public static void ThrowIfNull([NotNull] object? argument, int statusCode)
        {
            if (argument is null) {
                throw new CustomException(statusCode, $"Argument: {argument} can't be null!");
            }
        }

        public static void ThrowIf(bool expression, int statusCode, string message)
        {
            if (expression)
            {
                throw new CustomException(statusCode, message);
            }
        }
    }
}
