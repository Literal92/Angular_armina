
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.ViewModels.Identity.Settings;
using shop.ViewModels.Reservation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PermissionParts;
using PermissionParts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace shop.Services
{
    public interface IGetUserClaimsService
    {
        Task<(bool isAdmin, List<int> idOrganizations, List<int> idProviders)> GetClaims(int userId) ;
    }
    public class GetUserClaimsService: IGetUserClaimsService
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IOptionsSnapshot<SiteSettings> _settingOptions;

        public GetUserClaimsService(IApplicationUserManager userManager, IOptionsSnapshot<SiteSettings> settingOptions)
        {
            _userManager = userManager;
            _settingOptions = settingOptions;
        }
        public async Task<(bool isAdmin, List<int> idOrganizations,  List<int> idProviders)> GetClaims(int userId)
        {
            var output = await _userManager.GetByUserId(id: userId, includes: new Expression<Func<User, object>>[] { c => c.Claims });

            if (output.UserName == _settingOptions.Value.AdminUserSeed.Username)
                return (isAdmin: true, idOrganizations: null, idProviders: default);

            var claims = output?.Claims;
            if (claims == null)
                return (isAdmin: false, idOrganizations: null, idProviders: default);

            var claimOrganization = claims.Where(c => c.ClaimType == "organization").ToList();
            var organizations = claimOrganization.Count()>0? claimOrganization.Select(c => int.Parse(c.ClaimValue)).ToList():null;

            var claimProvider = claims.Where(c => c.ClaimType == "provider").ToList();
            var providers = claimProvider.Count() > 0 ? claimProvider.Select(c => int.Parse(c.ClaimValue)).ToList() : null;

            return (isAdmin: false, idOrganizations: organizations, idProviders: providers);

            
        }
    }

}






