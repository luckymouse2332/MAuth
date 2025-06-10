using System.ComponentModel.DataAnnotations;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Validators;

namespace MAuth.WebAPI.Controllers.Users.Requests;

public class UserUpdateRequest
{
    [Display(Name = "用户名")]
    [Required(ErrorMessage = "{0}是必填的")]
    [MaxLength(50, ErrorMessage = "{0}的长度不能大于{1}")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "密码")]
    [MaxLength(50, ErrorMessage = "{0}的长度不能大于{1}")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "角色")]
    [EnumValueValidation(typeof(UserRole))]
    public string Role { get; set; } = string.Empty;

    [Display(Name = "状态")]
    [EnumValueValidation(typeof(UserStatus))]
    public string Status { get; set; } = string.Empty;
}
