using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAuth.Web.Commons.Binders
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromAuthUserAttribute : ModelBinderAttribute
    {
        public FromAuthUserAttribute(string property) : base(typeof(AuthUserModelBinder))
        {
            Property = property;
            BindingSource = BindingSource.Custom;
        }

        public string Property { get; set; }
    }
}
