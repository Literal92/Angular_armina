using shop.ViewModels.Identity.Settings;
using Microsoft.EntityFrameworkCore;

namespace shop.DataLayer.Mappings
{
    public static class IdentityMappings
    {
        public static void AddCustomIdentityMappings(this ModelBuilder modelBuilder, SiteSettings siteSettings)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityMappings).Assembly);

            // IEntityTypeConfiguration's which have constructors with parameters
            modelBuilder.ApplyConfiguration(new AppSqlCacheConfiguration(siteSettings));
        }
    }
}