using shop.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using shop.Common.EFCoreToolkit;

namespace shop.DataLayer.SQLite
{
    public class SQLiteDbContext : ApplicationDbContext
    {
        public SQLiteDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.AddDateTimeOffsetConverter();
        }
    }
}