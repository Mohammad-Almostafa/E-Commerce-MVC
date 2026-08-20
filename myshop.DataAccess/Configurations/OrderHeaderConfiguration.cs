using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using myshop.DataAccess.Entities.Models;

namespace myshop.DataAccess.Configurations
{
    public class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}