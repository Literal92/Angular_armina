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
    public class UserClaimRequirementFilterAttribute : TypeFilterAttribute
    {

        public UserClaimRequirementFilterAttribute(string claimeType, string claimValue) : base(typeof(UserClaimRequirementFilter))
        {
            Arguments = new object[] { claimeType, claimValue };
        }
    }

    public class UserClaimRequirementFilter : IAsyncActionFilter
    {
        private readonly IApplicationUserManager _user;

        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly string _claimType;
        private readonly string _claimValue;

        public UserClaimRequirementFilter(IApplicationUserManager user,
            string claimeType, string claimValue,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions)
        {
            _user = user;
            _claimType = claimeType;
            _claimValue = claimValue;
            _adminUserSeedOptions = adminUserSeedOptions;
        }



        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           

            var param = context.ActionArguments[_claimValue];
            // var y = context.RouteData.Values[_claimValue];
            if (param == default(object))
            {
                // startegy ???
                //context.Result = new BadRequestResult();
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ForbidResult();
                return;
            }
            //custome area
            var userData = context?.HttpContext?.User?
                                .FindFirst(ClaimTypes.UserData)?.Value;

            if (userData == null)
            {
                context.HttpContext.Response.StatusCode = 401;

                context.Result = new UnauthorizedResult();
                return;
            }
            var user = await _user.FindByIdAsync(userData);
            // if superadmin
            if (user.UserName == _adminUserSeedOptions.Value.AdminUserSeed.Username)
            {
                await next();
                return;
            }

            var userClaim = await _user.GetClaimsAsync(user);
            var claimValue = param.ToString();
            bool existUserClaim = false;
            if (claimValue == "0")
            {
                await next();
                return;
            }
            else
            {
                existUserClaim = userClaim.ToList().Any(x =>
                                (x.Type).ToLower() == (_claimType).ToLower()
                                && (x.Value).ToLower() == (claimValue).ToLower());
            }

            if (!existUserClaim && claimValue != "0")
            {
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new ForbidResult("عدم دسترسی به این بخش");
            }

            await next();
           
        }

    }

}






