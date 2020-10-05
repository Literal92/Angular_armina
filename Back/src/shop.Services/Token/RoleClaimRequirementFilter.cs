using shop.Services.Contracts.Identity;

using shop.ViewModels.Identity.Settings;
using shop.ViewModels.Reservation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PermissionParts;
using PermissionParts.Enum;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace shop.Services
{
    // More Info: https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core
    // More Info : https://code-maze.com/action-filters-aspnetcore/
    // More Info : https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
    // You can try it yourself
    public class RoleClaimRequirementFilterAttribute : TypeFilterAttribute
    {

        public RoleClaimRequirementFilterAttribute(Permissions permission) : base(typeof(RoleClaimRequirementFilter))
        {
            Arguments = new object[] { permission };
        }
    }
    public class RoleClaimRequirementFilter : IAsyncActionFilter
    {
        //private readonly IRolesService _role;
        private readonly IApplicationRoleManager _role;
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly Permissions _permissions;

        public RoleClaimRequirementFilter(IApplicationRoleManager role,
            Permissions permissions,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions)
        {
            _role = role;
            _permissions = permissions;
            _adminUserSeedOptions = adminUserSeedOptions;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var permit = PermissionDisplay.GetPermissionToDisplay(enumType: typeof(Permissions), permissionName: _permissions.ToString());

                var userData = context?.HttpContext?.User?
                               .FindFirst(ClaimTypes.UserData)?.Value;

                var tryInt = int.TryParse(userData, out int userId);

                if (!tryInt)
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var roles = await _role.FindUserRolesAsync(userId);
                // if superadmin
                if (roles.Any(c=>c.Name.ToLower() == _adminUserSeedOptions.Value.AdminUserSeed.RoleName.ToLower()))
                {
                    await next();
                    return;
                }

                var roleClaims = context?.HttpContext?.User?.FindAll("RoleClaims").Select(o => JsonConvert.DeserializeObject<ClaimCustomeViewModel>(o.Value.ToString()));//.ToList();

                if (!roleClaims.Any())
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new ForbidResult();
                    return;
                }
                var exist = roleClaims.Any(c => c.Type.ToLower() == permit.GroupName.ToLower() && c.Value.ToLower() == permit.ShortName.ToLower());
                if (!exist)
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new ForbidResult();
                    return;
                }

            }
            catch (Exception ex)
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
            return;

        }

    }

}






