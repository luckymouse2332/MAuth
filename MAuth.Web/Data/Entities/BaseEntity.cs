using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAuth.Web.Data.Entities
{
    public class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public Guid Id { get; set; }

        public DateTimeOffset CreationTime { get; set; }

        public string? Modifier { get; set; }

        public DateTimeOffset? ModificationTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset DeletedTime { get; set; }
    }
}
