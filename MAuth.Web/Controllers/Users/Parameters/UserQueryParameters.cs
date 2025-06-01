using MAuth.Web.Commons.Models;

namespace MAuth.Web.Controllers.Users.Parameters;

public class UserQueryParameters : QueryParameterBase
{
    public string? Status { get; set; }

    public string? Role { get; set; }
}
