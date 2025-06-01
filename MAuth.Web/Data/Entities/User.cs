using MAuth.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Data.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(512)]
        public string Password { get; set; } = string.Empty;

        public UserStatus Status { get; set; } = UserStatus.Active;

        public UserRole Role { get; set; }

        public ICollection<Player> Players { get; set; } = [];
    }
}
