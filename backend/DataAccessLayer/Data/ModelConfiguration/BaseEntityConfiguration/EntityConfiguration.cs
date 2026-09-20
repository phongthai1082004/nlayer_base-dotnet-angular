using DataAccessLayer.Entities;
using DataAccessLayer.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Data.ModelConfiguration.BaseEntityConfiguration
{
    public static class EntityConfiguration
    {
        public static EntityTypeBuilder<T> ConfigureGuidEntityBase<T>(this EntityTypeBuilder<T> builder) where T : GuidEntityBase
        {
            builder.HasKey(e => e.Id);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.ModifiedAt).IsRequired(false);
            builder.Property(x => x.ModifiedBy).IsRequired(false);
            builder.Property(x => x.IsDeleted);
            builder.Property(x => x.DeletedAt).IsRequired(false);
            builder.Property(x => x.DeletedBy).IsRequired(false);
            return builder;
        }

        public static EntityTypeBuilder<T> ConfigureEntityBase<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
        {
            builder.HasKey(e => e.Id);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.ModifiedAt).IsRequired(false);
            builder.Property(x => x.ModifiedBy).IsRequired(false);
            return builder;
        }
    }
}
