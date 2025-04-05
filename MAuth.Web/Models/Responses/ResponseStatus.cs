using System.Net;

namespace MAuth.Web.Models.Responses
{
    /// <summary>
    /// 响应状态
    /// </summary>
    public record ResponseStatus(int Code, string Message)
    {
        public static readonly ResponseStatus Success = new(StatusCodes.Status200OK, "成功！");

        public static readonly ResponseStatus NotFound = new(StatusCodes.Status404NotFound, "未找到资源！");

        public static readonly ResponseStatus ParamError = new(StatusCodes.Status400BadRequest, "数据格式错误！");

        public static readonly ResponseStatus NoAuthError = new(StatusCodes.Status401Unauthorized, "未认证！");

        public static readonly ResponseStatus NoPermissionError = new(StatusCodes.Status403Forbidden, "权限不足！");

        public static readonly ResponseStatus InternalServerError = new(StatusCodes.Status500InternalServerError, "服务器内部错误！");
    }
}
