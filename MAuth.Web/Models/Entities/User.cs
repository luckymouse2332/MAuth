using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Models.Entities
{
    public class User : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(512)]
        public string Password { get; set; } = string.Empty;

        public UserStatus Status { get; set; }

        public UserRole Role { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Banned
    }

    public enum UserRole
    {
        Admin,
        User
    }
}
