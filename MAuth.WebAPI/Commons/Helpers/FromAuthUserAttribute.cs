using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAuth.WebAPI.Commons.Helpers;

[AttributeUsage(AttributeTargets.Parameter)]
public class FromAuthUserAttribute : ModelBinderAttribute
{
    protected FromAuthUserAttribute(string property) : base(typeof(AuthUserModelBinder))
    {
        Property = property;
    }

    public string Property { get; set; }

    public override BindingSource? BindingSource { get; protected set; } = BindingSource.Custom;
}