using MAuth.Web.Commons.Models;

namespace MAuth.Web.Controllers.Players.Parameters;

public class PlayerQueryParameter : QueryParameterBase
{
    public string? Status { get; set; }
}
