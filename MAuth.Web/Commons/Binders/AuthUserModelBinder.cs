using MAuth.Web.Commons.Helpers;
using MAuth.Web.Services.Identity;
using MAuth.Web.Services.Users;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace MAuth.Web.Commons.Binders
{
    public class AuthUserModelBinder(IUserRepository userRepository) : IModelBinder
    {
        private readonly IUserRepository userRepository = userRepository 
            ?? throw new ArgumentNullException(nameof(userRepository));

        public async Task BindModelAsync(ModelBindingContext context)
        {
            // 这里代表其他的ModelBinder已经处理过了
            if (context.BindingSource != BindingSource.Custom)
            {
                context.Result = ModelBindingResult.Failed();
                return;
            }

            var descriptor = context.HttpContext.GetEndpoint()
                ?.Metadata.GetMetadata<ControllerActionDescriptor>();

            var parameter = descriptor?.MethodInfo.GetParameters()
                .FirstOrDefault(p => p.Name == context.FieldName);

            var attr = parameter?.GetCustomAttribute<FromAuthUserAttribute>(true);
            if (attr == null)
            {
                context.Result = ModelBindingResult.Failed();
                return;
            }

            var id = IdentityHelper.GetUserIdFromPrincipal(context.HttpContext.User);

            var user = id.HasValue
                ? await userRepository.GetUserByIdAsync(id.Value)
                : null;

            var value = string.IsNullOrEmpty(attr.Property)
                ? user
                : user?.GetType().GetProperty(attr.Property)?.GetValue(user);

            context.Result = value != null && context.ModelType.IsInstanceOfType(value)
                ? ModelBindingResult.Success(value)
                : ModelBindingResult.Failed();
        }
    }
}
