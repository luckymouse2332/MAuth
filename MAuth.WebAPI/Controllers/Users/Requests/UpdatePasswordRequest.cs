using System.ComponentModel.DataAnnotations;

namespace MAuth.WebAPI.Controllers.Users.Requests;

/// <summary>
/// 更新密码的请求数据
/// </summary>
public class UpdatePasswordRequest
{
    [Required(ErrorMessage = "{0}是必填的")]
    [Display(Name = "旧密码")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
    
    [Required(ErrorMessage = "{0}是必填的")]
    [Display(Name = "新密码")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
}