using MAuth.Web.Commons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MAuth.Web.Commons.Filters
{
    public class ResponseWrapperFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult result)
            {
                // 如果返回结果已经是ResponseResult<T>类型的则不需要进行再次包装了
                if (result is { DeclaredType.IsGenericType: true }
                    && result.DeclaredType?.GetGenericTypeDefinition() == typeof(ResponseResult<>))
                {
                    return;
                }
                context.Result = new ObjectResult(ResponseResult.Success(result.Value));
            }
        }
    }
}
