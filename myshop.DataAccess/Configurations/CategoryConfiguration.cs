using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.DataAccess.Entities;

namespace myshop.DataAccess.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired();

            // Global Query Filter للـ Soft Delete
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}