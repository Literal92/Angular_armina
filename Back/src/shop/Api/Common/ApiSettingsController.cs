using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using shop.Common.GuardToolkit;
using System;

namespace shop.Controllers
{
    [Route("api/Common/[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiExplorerSettings(GroupName = "v3.0")]


    //[EnableCors("CorsPolicy")]
    public class ApiSettingsController : Controller
    {
        private readonly IOptionsSnapshot<ApiSettingsOptions> _apiSettingsConfig;


        public ApiSettingsController(IOptionsSnapshot<ApiSettingsOptions> apiSettingsConfig)
        {
            _apiSettingsConfig = apiSettingsConfig;
            _apiSettingsConfig.CheckArgumentIsNull(nameof(apiSettingsConfig));

        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_apiSettingsConfig.Value.ApiSettings); // For the Angular Client
        }
     
    }
}