
using shop.Entities;
using shop.Entities.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace shop.DataLayer.Mappings
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasOne(x => x.Parent)
                      .WithMany(x => x.SubCategory)
                      .HasForeignKey(x => x.ParentId);           
        }
    }
}