using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAuth.Web.Data.Entities
{
    public class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public Guid Id { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now;

        public string? Modifier { get; set; }

        public DateTime? ModificationTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DeletedTime { get; set; }
    }
}
