using DataAccessLayer.Data.ModelConfiguration.BaseEntityConfiguration;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.ModelConfiguration
{
    public class ExternalTokenConfiguration : IEntityTypeConfiguration<ExternalToken>
    {
        public void Configure(EntityTypeBuilder<ExternalToken> builder)
        {
            builder.ToTable("ExternalTokens");
            builder.ConfigureEntityBase();

            builder.Property(t => t.Provider)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.ProviderKey)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.ProviderDisplayName)
                .HasMaxLength(100);

            builder.Property(t => t.AccessToken)
                .HasColumnType("text");

            builder.Property(t => t.RefreshToken)
                .HasColumnType("text");

            builder.HasIndex(t => new { t.Provider, t.ProviderKey })
                .IsUnique();

            builder.HasOne(t => t.User)
                .WithMany(u => u.ExternalTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
