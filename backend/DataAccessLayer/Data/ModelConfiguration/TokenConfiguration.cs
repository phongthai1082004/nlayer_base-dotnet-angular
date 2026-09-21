using DataAccessLayer.Data.ModelConfiguration.BaseEntityConfiguration;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.ModelConfiguration
{
    public class TokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.ConfigureEntityBase();

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(t => t.Token)
                .IsUnique();

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(t => t.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
