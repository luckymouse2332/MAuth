using System.ComponentModel.DataAnnotations;

namespace MAuth.Web.Data.Entities
{
    public class Player : BaseEntity
    {
        public Guid UserId { get; set; }

        public string UUID { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public PlayerStatus Status { get; set; }
    }
}
