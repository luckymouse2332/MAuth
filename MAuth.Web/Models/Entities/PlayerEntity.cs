using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Models.Entities
{
    public class PlayerEntity : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public string UUID { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PlayerStatus Status { get; set; }
    }

    public enum PlayerStatus
    {
        Active,
        Banned
    }
}
