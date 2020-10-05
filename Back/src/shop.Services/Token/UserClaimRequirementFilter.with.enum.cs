//using shop.Services.Contracts.Identity;

//using shop.ViewModels.Identity.Settings;
//using shop.ViewModels.Reservation;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using PermissionParts;
//using PermissionParts.Enum;
//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace shop.Services
//{
//    // More Info: https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
//    // More Info : https://code-maze.com/action-filters-aspnetcore/
//    // More Info : https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
//    // You can try it yourself
//    public class UserClaimRequirementFilterAttribute : TypeFilterAttribute
//    {

//        public UserClaimRequirementFilterAttribute(Permissions permission) : base(typeof(UserClaimRequirementFilter))
//        {
//            Arguments = new object[] { permission };
//        }
//    }
//    public class UserClaimRequirementFilter : IAsyncActionFilter
//    {
//        //private readonly IRolesService _role;
//        private readonly IApplicationUserManager _user;
//        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
//        private readonly Permissions _permissions;

//        public UserClaimRequirementFilter(IApplicationUserManager user,
//            Permissions permissions,
//            IOptionsSnapshot<SiteSettings> adminUserSeedOptions)
//        {
//            _user = user;
//            _permissions = permissions;
//            _adminUserSeedOptions = adminUserSeedOptions;
//        }

//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            try
//            {
//                var permit = PermissionDisplay.GetPermissionToDisplay(enumType: typeof(Permissions), permissionName: _permissions.ToString());

//                var userData = context?.HttpContext?.User?
//                               .FindFirst(ClaimTypes.UserData)?.Value;

//                var tryInt = int.TryParse(userData, out int userId);

//                if (!tryInt)
//                {
//                    context.HttpContext.Response.StatusCode = 401;
//                    context.Result = new UnauthorizedResult();
//                    return;
//                }
//                var user = await _user.FindByIdAsync(userId.ToString());
//                // if superadmin
//                if (user.UserName == _adminUserSeedOptions.Value.AdminUserSeed.Username)
//                {
//                    await next();
//                    return;
//                }

//                var userClaims = context?.HttpContext?.User?.FindAll("UserClaims").Select(o => JsonConvert.DeserializeObject<ClaimCustomeViewModel>(o.Value.ToString()));//.ToList();

//                if (!userClaims.Any())
//                {
//                    context.HttpContext.Response.StatusCode = 403;
//                    context.Result = new ForbidResult();
//                    return;
//                }
//                var exist = userClaims.Any(c => c.Type.ToLower() == permit.GroupName.ToLower() && c.Value.ToLower() == permit.ShortName.ToLower());
//                if (!exist)
//                {
//                    context.HttpContext.Response.StatusCode = 403;
//                    context.Result = new ForbidResult();
//                    return;
//                }

//            }
//            catch (Exception ex)
//            {
//                context.HttpContext.Response.StatusCode = 401;
//                context.Result = new UnauthorizedResult();
//                return;
//            }
//            await next();
//            return;

//        }

//    }

//}






