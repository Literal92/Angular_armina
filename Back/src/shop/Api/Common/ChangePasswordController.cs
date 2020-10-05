using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using shop.Common.GuardToolkit;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity;

namespace shop.Controllers
{
    [Authorize]
    [Route("api/Common/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiExplorerSettings(GroupName = "v3.0")]

    public class ChangePasswordController : Controller
    {
        private readonly IApplicationUserManager _usersService;
        public ChangePasswordController(IApplicationUserManager usersService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _usersService.GetCurrentUserAsync();
            if (user == null)
            {
                return BadRequest("NotFound");
            }
            var t = false;
            if (t==false)
            {
                return NotFound("پیدا نشد.");
            }

            var result = await _usersService.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
    }
}