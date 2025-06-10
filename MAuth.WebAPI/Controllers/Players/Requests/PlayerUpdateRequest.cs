using System.ComponentModel.DataAnnotations;
using MAuth.Data.Entities;

namespace MAuth.WebAPI.Controllers.Players.Requests;

public class PlayerUpdateRequest
{
    [Display(Name = "玩家UUID")]
    [Required(ErrorMessage = "{0}是必填的")]
    public string UUID { get; set; } = string.Empty;

    [Display(Name = "玩家名")]
    [Required(ErrorMessage = "{0}是必填的")]
    [MaxLength(255, ErrorMessage = "{0}的长度必须短于{1}")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "玩家状态")]
    [EnumDataType(typeof(PlayerStatus), ErrorMessage = "无效的玩家状态")]
    public string Status { get; set; } = string.Empty;
}
