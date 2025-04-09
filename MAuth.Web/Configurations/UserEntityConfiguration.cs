using MAuth.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MAuth.Web.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.Property(x => x.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<UserStatus>(v));

            builder.Property(x => x.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<UserRole>(v));

            builder.HasMany(x => x.Players)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}
