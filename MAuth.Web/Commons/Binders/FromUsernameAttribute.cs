namespace MAuth.Web.Commons.Binders
{
    public class FromUsernameAttribute : FromAuthUserAttribute
    {
        public FromUsernameAttribute() : base("Username")
        {
        }
    }
}
