using MAuth.WebAPI.Commons.Models;

namespace MAuth.WebAPI.Controllers.Players.Parameters;

public class PlayerQueryParameter : QueryParameterBase
{
    public string? Status { get; set; }
}
