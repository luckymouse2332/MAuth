using MAuth.Contracts.Enums;
using MAuth.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAuth.Web.Data.Configurations
{
    public class PlayerEntityConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("players");

            builder.Property(x => x.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<PlayerStatus>(v));
        }
    }
}
