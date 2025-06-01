using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Controllers.Players.Requests;

public class PlayerCreateRequest
{
    [Display(Name = "玩家UUID")]
    [Required(ErrorMessage = "{0}是必填的")]
    public string UUID { get; set; } = string.Empty;

    [Display(Name = "玩家名")]
    [Required(ErrorMessage = "{0}是必填的")]
    [MaxLength(255, ErrorMessage = "{0}的长度必须短于{1}")]
    public string Name { get; set; } = string.Empty;
}
