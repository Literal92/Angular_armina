using System.Security.Claims;
using System.Security.Principal;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Services;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.Services.Identity;
using shop.Services.Identity.Logger;
using shop.Services.Reservation;
using shop.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace shop.IocConfig
{
    public static class AddCustomServicesExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IPrincipal>(provider =>
                provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User ?? ClaimsPrincipal.Current);

            services.AddScoped<ILookupNormalizer, CustomNormalizer>();

            services.AddScoped<ISecurityStampValidator, CustomSecurityStampValidator>();
            services.AddScoped<SecurityStampValidator<User>, CustomSecurityStampValidator>();

            services.AddScoped<IPasswordValidator<User>, CustomPasswordValidator>();
            services.AddScoped<PasswordValidator<User>, CustomPasswordValidator>();

            services.AddScoped<IUserValidator<User>, CustomUserValidator>();
            services.AddScoped<UserValidator<User>, CustomUserValidator>();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, ApplicationClaimsPrincipalFactory>();
            services.AddScoped<UserClaimsPrincipalFactory<User, Role>, ApplicationClaimsPrincipalFactory>();

            services.AddScoped<IdentityErrorDescriber, CustomIdentityErrorDescriber>();

            services.AddScoped<IApplicationUserStore, ApplicationUserStore>();
            services.AddScoped<UserStore<User, Role, ApplicationDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>, ApplicationUserStore>();

            services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
            services.AddScoped<UserManager<User>, ApplicationUserManager>();

            services.AddScoped<IApplicationRoleManager, ApplicationRoleManager>();
            services.AddScoped<RoleManager<Role>, ApplicationRoleManager>();

            services.AddScoped<IApplicationSignInManager, ApplicationSignInManager>();
            services.AddScoped<SignInManager<User>, ApplicationSignInManager>();

            services.AddScoped<IApplicationRoleStore, ApplicationRoleStore>();
            services.AddScoped<RoleStore<Role, ApplicationDbContext, int, UserRole, RoleClaim>, ApplicationRoleStore>();

            //services.AddScoped<IEmailSender, AuthMessageSender>();
            //services.AddScoped<ISmsSender, AuthMessageSender>();

            services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();
            services.AddScoped<IUsedPasswordsService, UsedPasswordsService>();
            services.AddScoped<ISiteStatService, SiteStatService>();
            services.AddScoped<IUsersPhotoService, UsersPhotoService>();
            //services.AddScoped<ISecurityTrimmingService, SecurityTrimmingService>();
            services.AddScoped<IAppLogItemsService, AppLogItemsService>();

            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenFactoryService, TokenFactoryService>();


            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddSingleton<ISecurityService, SecurityService>();

          //  services.AddSingleton<IMvcActionsDiscoveryService, MvcActionsDiscoveryService>();

            #region Shop
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();


            
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IUserDeviceService, UserDeviceService>();
            // services.AddScoped<IGetUserClaimsService, GetUserClaimsService>();            
            services.AddScoped<IFieldService, FieldService>();            
            services.AddScoped<IProductOptionService, ProductOptionService>();    
            services.AddScoped<IOptionColorService, OptionColorService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IOrderProductService, OrderProductService>();
            services.AddScoped<IGateWayService, GateWayService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<IImageDateService, ImageDateService>();

            services.AddScoped<ITrackingCodeService, TrackingCodeService>();
            services.AddScoped<ITagService, TagService>();

            #endregion



            return services;
        }
    }
}