using MAuth.Web.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Models.DTOs
{
    public class UserCreateDto
    {
        [Display(Name = "用户名")]
        [Length(5, 50, ErrorMessage = "{0}的长度必须介于{1}和{2}")]
        [Required]
        public required string Username { get; set; }

        [Display(Name = "密码")]
        [Length(6, 50, ErrorMessage = "{0}的长度必须介于{1}和{2}")]
        [Required]
        public required string Password { get; set; }

        [Display(Name = "角色")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "{0}的值必须是{1}或{2}")]
        public required string Role { get; set; }
    }
}
