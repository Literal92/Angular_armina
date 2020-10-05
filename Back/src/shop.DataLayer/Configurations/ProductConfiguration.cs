
using shop.Entities;
using shop.Entities.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace shop.DataLayer.Mappings
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(x => x.Parent)
                      .WithMany(x => x.ChildProduct)
                      .HasForeignKey(x => x.ParentId);
        }
    }
}