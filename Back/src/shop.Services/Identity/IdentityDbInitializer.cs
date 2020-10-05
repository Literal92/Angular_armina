using System;
using System.Linq;
using System.Threading.Tasks;
using DNTPersianUtils.Core.IranCities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using shop.Common.GuardToolkit;
using shop.Common.IdentityToolkit;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.ViewModels.Identity.Settings;

namespace shop.Services.Identity
{
    public class IdentityDbInitializer : IIdentityDbInitializer
    {
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly ILogger<IdentityDbInitializer> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ICategoryService _categoryService;
        public IdentityDbInitializer(
            IApplicationUserManager applicationUserManager,
            IServiceScopeFactory scopeFactory,
            IApplicationRoleManager roleManager,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions,
            ICategoryService categoryService,
            ILogger<IdentityDbInitializer> logger
        )
        {
            _applicationUserManager = applicationUserManager;
            _applicationUserManager.CheckArgumentIsNull(nameof(_applicationUserManager));

            _scopeFactory = scopeFactory;
            _scopeFactory.CheckArgumentIsNull(nameof(_scopeFactory));

            _roleManager = roleManager;
            _roleManager.CheckArgumentIsNull(nameof(_roleManager));

            _adminUserSeedOptions = adminUserSeedOptions;
            _adminUserSeedOptions.CheckArgumentIsNull(nameof(_adminUserSeedOptions));

            _categoryService = categoryService;

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
        }

        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// Will create the database if it does not already exist.
        /// </summary>
        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    if (_adminUserSeedOptions.Value.ActiveDatabase == ActiveDatabase.InMemoryDatabase)
                    {
                        context.Database.EnsureCreated();
                    }
                    else
                    {
                        context.Database.Migrate();
                    }
                }
            }
        }

        /// <summary>
        /// Adds some default values to the IdentityDb
        /// </summary>
        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                var identityDbSeedData = serviceScope.ServiceProvider.GetRequiredService<IIdentityDbInitializer>();
                var result = identityDbSeedData.SeedDatabaseWithAdminUserAsync().Result;
                if (result == IdentityResult.Failed())
                {
                    throw new InvalidOperationException(result.DumpErrors());
                }

                // How to add initial data to the DB directly
                using (var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    if (!context.Roles.Any())
                    {
                        context.Add(new Role(ConstantRoles.SuperAdmin));

                        context.Add(new Role { Name = ConstantRoles.Admin, Description = "ادمین" });

                        context.Add(new Role { Name = ConstantRoles.Client, Description = "مشتری" });

                        context.Add(new Role { Name = ConstantRoles.ProductAdmin, Description = "مدیر محصولات" });

                        context.Add(new Role { Name = ConstantRoles.OrderAdmin, Description = "مدیر سفارشات" });

                        context.Add(new Role { Name = ConstantRoles.ClientAdmin, Description = "مدیر فروشنده" });

                        context.Add(new Role { Name = ConstantRoles.AccountantAdmin, Description = "مدیر مالی" });

                        context.Add(new Role { Name = ConstantRoles.ReportAdmin, Description = "مدیر گزارشات" });

                        context.SaveChanges();
                    }
                }
            }
        }

        public async Task<IdentityResult> SeedDatabaseWithAdminUserAsync()
        {
            #region check admin
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            adminUserSeed.CheckArgumentIsNull(nameof(adminUserSeed));

            var name = adminUserSeed.Username;
            var password = adminUserSeed.Password;
            var email = adminUserSeed.Email;
            var roleName = adminUserSeed.RoleName;

            var thisMethodName = nameof(SeedDatabaseWithAdminUserAsync);

            var adminUser = await _applicationUserManager.FindByNameAsync(name);
            if (adminUser == null)
            {

                //Create the `Admin` Role if it does not exist
                var adminRole = await _roleManager.FindByNameAsync(roleName);
                if (adminRole == null)
                {
                    adminRole = new Role(roleName);
                    var adminRoleResult = await _roleManager.CreateAsync(adminRole);
                    if (adminRoleResult == IdentityResult.Failed())
                    {
                        _logger.LogError($"{thisMethodName}: adminRole CreateAsync failed. {adminRoleResult.DumpErrors()}");
                        return IdentityResult.Failed();
                    }
                }
                //else
                //{
                //    _logger.LogInformation($"{thisMethodName}: adminRole already exists.");
                //}

                adminUser = new User
                {
                    UserName = name,
                    Email = email,
                    EmailConfirmed = true,
                    IsEmailPublic = true,
                    SerialNumber = Guid.NewGuid().ToString()
                    //LockoutEnabled = true
                };
                var adminUserResult = await _applicationUserManager.CreateAsync(adminUser, password);
                if (adminUserResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminUser CreateAsync failed. {adminUserResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }

                var setLockoutResult = await _applicationUserManager.SetLockoutEnabledAsync(adminUser, enabled: false);
                if (setLockoutResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminUser SetLockoutEnabledAsync failed. {setLockoutResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }

                var addToRoleResult = await _applicationUserManager.AddToRoleAsync(adminUser, adminRole.Name);
                if (addToRoleResult == IdentityResult.Failed())
                {
                    _logger.LogError($"{thisMethodName}: adminUser AddToRoleAsync failed. {addToRoleResult.DumpErrors()}");
                    return IdentityResult.Failed();
                }
            }
            #endregion
            #region Category
            var root = await _categoryService.GetByTitle("ریشه");
            if (root == null)
            {
                var output = await _categoryService.Create(new Category
                {
                    Title = "ریشه",
                    Path = "ریشه"
                });
                if (!output.success)
                {
                    _logger.LogError($"{thisMethodName}: root for category CreateAsync failed. {output.error}");
                    return IdentityResult.Failed();
                }
            }

            #endregion


            return IdentityResult.Success;
        }
    }
}