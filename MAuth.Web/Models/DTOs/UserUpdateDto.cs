using MAuth.Web.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Models.DTOs
{
    public class UserUpdateDto
    {
        [Display(Name = "用户名")]
        [MaxLength(50, ErrorMessage = "{0}的长度不能大于{1}")]
        public string? Username { get; set; }

        [Display(Name = "密码")]
        [MaxLength(50, ErrorMessage = "{0}的长度不能大于{1}")]
        public string? Password { get; set; }

        [Display(Name = "角色")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "{0}的值必须为{1}或{2}")]
        public string? Role { get; set; }

        [Display(Name = "状态")]
        [EnumDataType(typeof(UserStatus), ErrorMessage = "{0}的值必须为{1}或{2}")]
        public string? Status { get; set; }
    }
}
