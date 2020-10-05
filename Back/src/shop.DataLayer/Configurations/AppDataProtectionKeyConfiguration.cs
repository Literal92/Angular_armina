using shop.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace shop.DataLayer.Mappings
{
    public class AppDataProtectionKeyConfiguration : IEntityTypeConfiguration<AppDataProtectionKey>
    {
        public void Configure(EntityTypeBuilder<AppDataProtectionKey> builder)
        {
            builder.ToTable("AppDataProtectionKeys");
            builder.HasIndex(e => e.FriendlyName).IsUnique();
        }
    }
}