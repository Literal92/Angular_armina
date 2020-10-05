using shop.Entities.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using shop.Services.Contracts.Identity;

namespace shop.Extension
{
    public static class IHttpContextAccessorExtension
    {
        public static string CurrentSerialNumber(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
        }
        //public static int UserId(this IHttpContextAccessor httpContextAccessor, IApplicationUserManager applicationUserManager)
        //{
        //    var serialNumber = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
        //    if (serialNumber == default)
        //        return 0;

        //    var user = applicationUserManager.FindBySerialNumber(serialNumber).Result;
        //    if (user == default)
        //        return 0;

        //    return user.Id;
        //}
        public static int UserId(this IHttpContextAccessor httpContextAccessor)
        {
            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.UserData)?.Value;
            if (userId == null)
                return 0;

            var parse= int.TryParse(userId, out int id);
            return parse == false ? 0 : id;
        }

        public static User User(this IHttpContextAccessor httpContextAccessor, IApplicationUserManager applicationUserManager)
        {
            var serialNumber = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.SerialNumber)?.Value;
            if (serialNumber == default)
                return null;

            var user = applicationUserManager.FindBySerialNumber(serialNumber).Result;
            if (user == default)
                return null;
            return user;
        }

        public static int CityId(this IHttpContextAccessor httpContextAccessor)
        {
            var CityId = httpContextAccessor?.HttpContext?.User?.FindFirst("CityId")?.Value;
            if (CityId == default)
                return 0;

            return Convert.ToInt32(CityId);
        }

        public static int AreaId(this IHttpContextAccessor httpContextAccessor)
        {
            var AreaId = httpContextAccessor?.HttpContext?.User?.FindFirst("AreaId")?.Value;
            if (AreaId == default)
                return 0;

            return Convert.ToInt32(AreaId);
        }

    }
}
