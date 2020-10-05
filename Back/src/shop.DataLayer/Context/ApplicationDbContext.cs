using shop.Common.GuardToolkit;
using shop.Entities.AuditableEntity;
using shop.Entities.Identity;
using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System;
using shop.DataLayer.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using shop.Common.EFCoreToolkit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using shop.Entities.Reservation;

namespace shop.DataLayer.Context
{
    public class ApplicationDbContext :
        IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IUnitOfWork
    {
        private IDbContextTransaction _transaction;

        // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        #region BaseClass

        public virtual DbSet<AppLogItem> AppLogItems { get; set; }
        public virtual DbSet<AppSqlCache> AppSqlCache { get; set; }
        public virtual DbSet<AppDataProtectionKey> AppDataProtectionKeys { get; set; }
        
        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().AddRange(entities);
        }

        public void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
            {
                throw new NullReferenceException("Please call `BeginTransaction()` method first.");
            }
            _transaction.Rollback();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                throw new NullReferenceException("Please call `BeginTransaction()` method first.");
            }
            _transaction.Commit();
        }


        public void ExecuteSqlInterpolatedCommand(FormattableString query)
        {
            Database.ExecuteSqlInterpolated(query);
        }

        public void ExecuteSqlRawCommand(string query, params object[] parameters)
        {
            Database.ExecuteSqlRaw(query, parameters);
        }

        public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
        {
            var value = this.Entry(entity).Property(propertyName).CurrentValue;
            return value != null
                ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                : default;
        }

        public object GetShadowPropertyValue(object entity, string propertyName)
        {
            return this.Entry(entity).Property(propertyName).CurrentValue;
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            Update(entity);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().RemoveRange(entities);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges(); //NOTE: changeTracker.Entries<T>() will call it automatically.

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges();
            ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChangesAsync(cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;
            return result;
        }

        private void beforeSaveTriggers()
        {
            validateEntities();
            setShadowProperties();
        }

        private void setShadowProperties()
        {
            // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
            var httpContextAccessor = this.GetService<IHttpContextAccessor>();
            ChangeTracker.SetAuditableEntityPropertyValues(httpContextAccessor);
        }

        private void validateEntities()
        {
            var errors = this.GetValidationErrors();
            if (!string.IsNullOrWhiteSpace(errors))
            {
                // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
                var loggerFactory = this.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }
        }

        #endregion

        #region DbSets

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<ProductOption> ProductOptions { get; set; }
        public virtual DbSet<OptionColor> OptionColors { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }        
        public virtual DbSet<GateWay> GateWays { get; set; }
        public virtual DbSet<TrackingCode> TrackingCodes { get; set; }
        public virtual DbSet<ImageDate> ImageDates { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ProductTag> ProductTags { get; set; }


        #endregion
        #region DbQuery
        // public DbQuery<ReserveForUserRawReportViewModel> ReserveForUserRaw { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
            // Adds all of the ASP.NET Core Identity related mappings at once.
            builder.AddCustomIdentityMappings(this.GetService<IOptionsSnapshot<SiteSettings>>()?.Value);

            // Custom application mappings
            builder.SetDecimalPrecision();
            builder.AddDateTimeUtcKindConverter();

            // more info: https://stackoverflow.com/questions/46497733/using-singular-table-names-with-ef-core-2
            // more info for core 2 and core 3: https://stackoverflow.com/questions/45812459/ef-core-2-apply-hasqueryfilter-for-all-entity
            foreach (var entityType in builder.Model.GetEntityTypes())
            {

                if (entityType.FindProperty("IsDeleted") != null)
                {


                    var parameter = Expression.Parameter(entityType.ClrType);
                    //var a = entityType.GetProperty("IsDeleted");
                   // var a = entityType.FindProperty("IsDeleted");

                    // EF.Property<bool>(post, "IsDeleted")
                    var propertyMethodInfo = typeof(EF).GetMethod("Property").MakeGenericMethod(typeof(bool));
                    var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));

                    // EF.Property<bool>(post, "IsDeleted") == false
                    BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));

                    // post => EF.Property<bool>(post, "IsDeleted") == false
                    var lambda = Expression.Lambda(compareExpression, parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);

                }

                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }
            // This should be placed here, at the end.
            builder.AddAuditableShadowProperties();

        }

        #region obsolete
        public DatabaseFacade db()
        {

            return Database;
        }
        public virtual IDbContextTransaction tran()
        {
            return Database.BeginTransaction();
        }
        public virtual async Task<IDbContextTransaction> tranAsync()
        {
            return await Database.BeginTransactionAsync();
        }
        public virtual IExecutionStrategy strategy()
        {
            return Database.CreateExecutionStrategy();
        }
        #endregion
        //private static Func<ApplicationDbContext, List<int>, IAsyncEnumerable<Provider>> _getProvider =
        //             EF.CompileAsyncQuery((ApplicationDbContext _ctx, List<int> ids) =>
        //             _ctx.Providers.Where(c => ids.Any(z => z == c.Id)).Include(c => c.User));

        // public async Task<List<Provider>> CompileQuryProvider(List<int> ids)
        // {
        //     var query = EF.CompileAsyncQuery((ApplicationDbContext _ctx, List<int> ids) =>
        //          _ctx.Providers.Where(c => ids.Any(z => z == c.Id)).Include(c => c.User));
        //     var t= await query(this, ids).Result.ToListAsync();
        //     return t; 
        // }

        // public async Task<List<ReserveForUserRawReportViewModel>> ExecuteReaderAsync<T>(string sqlQuery) where T : class
        // {
        //     try
        //     {
        //        var output= await this.ReserveForUserRaw.FromSqlRaw(sqlQuery).ToListAsync();
        //         return output;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }
        public override void Dispose()
        {
            _transaction?.Dispose();
            base.Dispose();
        }

    }

        

    
}