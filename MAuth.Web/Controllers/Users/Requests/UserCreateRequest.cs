using MAuth.Contracts.Enums;
using MAuth.Web.Controllers.Players.Requests;
using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Controllers.Users.Requests;

public class UserCreateRequest
{
    [Display(Name = "用户名")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "{0}的长度必须介于{2}和{1}之间")]
    [Required(ErrorMessage = "{0}是必填项")]
    public required string Username { get; set; }

    [Display(Name = "密码")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "{0}的长度必须介于{2}和{1}之间")]
    [Required(ErrorMessage = "{0}是必填项")]
    public required string Password { get; set; }

    [Display(Name = "角色")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "{0}的值必须是有效的角色")]
    [Required(ErrorMessage = "{0}是必填项")]
    public required string Role { get; set; }

    public ICollection<PlayerCreateRequest> Players { get; set; } = [];
}
