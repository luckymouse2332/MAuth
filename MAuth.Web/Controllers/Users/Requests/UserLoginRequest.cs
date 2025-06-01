using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Controllers.Users.Requests;

public class UserLoginRequest
{
    [Display(Name = "用户名")]
    [Required(ErrorMessage = "字段{0}是必填的")]
    public required string Username { get; set; }

    [Display(Name = "密码")]
    [Required(ErrorMessage = "字段{0}是必填的")]
    public required string Password { get; set; }
}
