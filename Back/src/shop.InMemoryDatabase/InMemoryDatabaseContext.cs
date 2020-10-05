using shop.DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace shop.DataLayer.InMemoryDatabase
{
    public class InMemoryDatabaseContext : ApplicationDbContext
    {
        public InMemoryDatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}