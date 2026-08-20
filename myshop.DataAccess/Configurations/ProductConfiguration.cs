using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                   .IsRequired();

            builder.Property(p => p.Price)
                    .IsRequired();

            builder.Property(p => p.CategoryId)
                    .IsRequired();

            // Global Query Filter للـ Soft Delete
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}