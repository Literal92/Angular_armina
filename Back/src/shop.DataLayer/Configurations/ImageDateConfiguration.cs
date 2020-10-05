
using shop.Entities;
using shop.Entities.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace shop.DataLayer.Mappings
{
    public class ImageDateConfiguration : IEntityTypeConfiguration<ImageDate>
    {
        public void Configure(EntityTypeBuilder<ImageDate> builder)
        {
            builder.Property(e => e.Images)
            .HasConversion(
                  v => string.Join(',', v),
                  v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}