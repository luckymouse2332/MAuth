using MAuth.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAuth.Web.Configurations
{
    public class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
    {
        public void Configure(EntityTypeBuilder<PlayerEntity> builder)
        {
            builder.ToTable("players");

            builder.Property(x => x.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<PlayerStatus>(v));
        }
    }
}
