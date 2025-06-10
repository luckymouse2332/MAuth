using MAuth.WebAPI.Commons.Models;

namespace MAuth.WebAPI.Controllers.Users.Parameters;

public class UserQueryParameters : QueryParameterBase
{
    public string? Status { get; set; }

    public string? Role { get; set; }
}
