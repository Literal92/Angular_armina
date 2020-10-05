using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Services;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using shop.Services.Identity;

namespace shop.Area.Api.Controllers
{
    //[Route("api/Admin/[controller]/[action]/{id?}")]
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Admin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    //[ClaimRequirement]
    //[Area("Admin")]
    //[ApiController]

    [DisplayName("داشبورد مدیر")]
    // [ApiExplorerSettings(IgnoreApi = true)]

    public class DashboardController : Controller //ControllerBase
    {
        #region Fields
        #endregion

        #region Ctor

        public DashboardController()
        {
        }
        #endregion

        #region Methods     
        [HttpGet]
        [DisplayName("نمایش")]
        public IActionResult Get()
        {
            return Ok(true);
        }

        #endregion
    }
}
