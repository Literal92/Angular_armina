//using shop.Entities;
//using shop.Services.Contracts.Identity;
//using shop.Services.Token;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace shop.Services
//{
//    // write by kamal
//    // More Info: https://stackoverflow.com/questions/31464359/how-do-you-create-a-custom-authorizeattribute-in-asp-net-core

//    public class UserClaimRequirementFilterOrginalAttribute : TypeFilterAttribute
//    {

//        public UserClaimRequirementFilterOrginalAttribute(string areaType) : base(typeof(UserClaimRequirementFilter))
//        {
//            Arguments = new object[] { areaType };
//        }
//    }

//    public class UserClaimRequirementFilterOrginal : IAuthorizationFilter
//    {
//        //private readonly IRolesService _role;
//        private readonly IApplicationUserManager _user;
//        private readonly string _areaType;

//        public UserClaimRequirementFilterOrginal(IApplicationUserManager user, string areaType)
//        {
//            _user = user;
//            _areaType = areaType;
//        }

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            #region UserClaim Check
//            //https://stackoverflow.com/questions/12817202/accessing-post-or-get-parameters-in-custom-authorization-mvc4-web-api/16941399
//            //context.ActionDescriptor.
//            //actionContext.ActionArguments["model"];
//            var t=context.RouteData.Values["id"];
//            var s=context.ActionDescriptor.Parameters;
//            var y = context.ActionDescriptor.Id;
//            var serialNumberClaim = context?.HttpContext?.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
//            if (serialNumberClaim == null)
//            {
//                context.Result = new ForbidResult();
//                return;
//            }
//            var user = _user.FindBySerialNumber(serialNumberClaim).Result;

//            var userClaim = _user.GetClaimsAsync(user).Result;

//            var existUserClaim = userClaim.ToList().Any(x => x.Type == _areaType);
//            if (!existUserClaim)
//            {
//                context.Result = new ForbidResult();
//                return;
//            }
//            // UserClaim Check

//            #endregion
//        }
//    }

   
//}






