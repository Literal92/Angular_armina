using shop.Services.Contracts.Identity;
using shop.Services.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using shop.Services.Identity;

namespace shop.Services
{
    // write by kamal
    // More Info: https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core

    public class ClaimRequirementAttribute : TypeFilterAttribute
    {

        public ClaimRequirementAttribute() : base(typeof(ClaimRequirementFilter))
        {
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        //private readonly IRolesService _role;
        private readonly IApplicationRoleManager _role;


        public ClaimRequirementFilter(IApplicationRoleManager role )
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var t1 = context.HttpContext.User as ClaimsPrincipal;
            //var t= context?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (context?.HttpContext?.User?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var helpeReq = context.ActionDescriptor;
            // for area 
            helpeReq.RouteValues.TryGetValue("area", out var areaName);
            var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;
            // for controller
            helpeReq.RouteValues.TryGetValue("controller", out var controllerName);
            var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

            // for action
            helpeReq.RouteValues.TryGetValue("action", out var actionName);
            var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;
            // for api
            var api = context.HttpContext.Request.Path.Value.StartsWith("/api") ? "api" : string.Empty;



            (string claimType, string claimValue) = (string.Empty, $"{action}");
            switch (area ?? string.Empty)
            {
                case "":
                    claimType = $"{controller}";
                    break;
                default:
                    claimType = $"{area}.{controller}";
                    break;
            }
            // if superAdmin
            if (context.HttpContext.User.IsInRole(ConstantRoles.SuperAdmin))
            {
                return;
            }
            var roles = context.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(a => a.Value).ToList();
            if (roles.Count() == 0)
            {
                context.Result = new ForbidResult();
                return;
            }
            #region RoleClaim Check
            // --------- check with read from db

            bool isClaim = false;
            foreach (var item in roles)
            {
                isClaim = _role.IsClaim(item, claimType, claimValue).Result;
                if (isClaim == true)
                {
                    return;
                }
            }
            if (isClaim == false)
            {
                context.Result = new ForbidResult();
                return;
            }

            // ---------------end checked

            // --------- check with read from context
            //var roleClaims = context.HttpContext?.User?.Claims
            //    .Where(c => c.Type == "RoleClaims")
            //    .ToList();
            //if (roleClaims.Count == 0)
            //{
            //    context.Result = new ForbidResult();
            //    return;
            //}
            //bool isPermission = false;
            //roleClaims.ForEach(s =>
            //{
            //    var claimContent = (JsonConvert.DeserializeObject<dynamic>(s.Value));
            //    if ((claimContent.Type).Value.ToLower() == claimType.ToLower() && (claimContent.Value).Value.ToLower() == claimValue.ToLower())
            //    {
            //        isPermission = true;
            //        return;
            //    }

            //});
            //if (isPermission != true)
            //{
            //    context.Result = new ForbidResult();
            //    return;
            //}
            #endregion

            //#region UserClaim Check

            //var serialNumberClaim = context?.HttpContext?.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
            //if (serialNumberClaim==null)
            //{
            //    context.Result = new ForbidResult();
            //    return;
            //}
            //var user = _user.FindBySerialNumber(serialNumberClaim).Result;

            //var userClaim = _user.GetClaimsAsync(user).Result;
            //var existUserClaim = userClaim.ToList().Any(x => x.Value == "");
            //if (!existUserClaim)
            //{
            //    context.Result = new ForbidResult();
            //    return;
            //}
            //// UserClaim Check

            //#endregion
        }
    }

}






