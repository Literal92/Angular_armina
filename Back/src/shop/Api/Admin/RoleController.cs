using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Services;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using shop.ViewModels.Identity;
using shop.Services.Identity;

namespace shop.Area.Api.Controllers
{
    //[Route("api/Admin/[controller]/[action]/{id?}")]
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Admin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    //[ClaimRequirement]
    //[Area("Admin")]
    //// [ApiController]

    //[DisplayName("نقش ها")]
    public class RoleController : Controller //ControllerBase
    {
        #region Fields
        private readonly IApplicationRoleManager _role;
        private readonly IUnitOfWork _uow;
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        #endregion

        #region Ctor
        public RoleController(IApplicationRoleManager role, IUnitOfWork uow,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions)
        {
            _role = role;
            _uow = uow;
            _adminUserSeedOptions = adminUserSeedOptions;
        }
        #endregion

        #region Methods



        [HttpGet]
        [DisplayName("نمایش نقش ها")]
        public async Task<(List<RoleCustomeViewModel> roles, int count)> GetNew()
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;

            var list = await _role.GetRolesAsync(
                 where: c => ((c.Name).ToLower() != (adminUserSeed.RoleName).ToLower()),
                 orderby: c => c.OrderBy(x => x.Id));

            var count = await _role.Count(c => ((c.Name).ToLower() != (adminUserSeed.RoleName).ToLower()));

            var roles = list.Select(c => new RoleCustomeViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();
            return (roles: roles, count: count);
        }


        [HttpPost]
        [DisplayName("نمایش نقش ها")]
        public async Task<(IEnumerable<Role> roles, int count)> Get([FromBody](int pageSize, int pageIndex) Inputs)
        {
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;

            var roles = await _role.GetRolesAsync(
                 where: c => ((c.Name).ToLower() != (adminUserSeed.RoleName).ToLower()),
                 orderby: c => c.OrderBy(x => x.Id),
                 skip: (Inputs.pageIndex - 1) * Inputs.pageSize,
                 take: Inputs.pageSize);
            var count = await _role.Count(c => ((c.Name).ToLower() != (adminUserSeed.RoleName).ToLower()));

            return (roles: roles, count: count);
        }
        [HttpGet("{id}")]
        [DisplayName("نمایش نقش های یک کاربر براساس شناسه کابر")]
        public async Task<(IEnumerable<Role> roles, IEnumerable<Role> selected)> GetRolesByUserId([FromRoute]string id)
        {
            List<Role> selectedRole = default;
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            if (int.TryParse(id, out int Id))
            {

                var userRole = await _role.GetUserRoleAsync(
                    where: c => ((c.Role.Name).ToLower() != (adminUserSeed.RoleName).ToLower()
                                    &&
                                    (c.UserId == Id)),
                    includeProperties: c => c.Role
                    );
                selectedRole = userRole.Select(a => a.Role).ToList();
                //selectedRole = userRole.Select(a=> new Role() { Id = a.Role.Id,
                //    Name =a.Role.Name,
                //NormalizedName=a.Role.NormalizedName,
                //Description=a.Role.Description,
                //Claims=a.Role.Claims,
                //ConcurrencyStamp =a.Role.ConcurrencyStamp} ).ToList();
            }
            var roles = await _role.GetRolesAsync(c => (c.Name).ToLower() != (adminUserSeed.RoleName).ToLower());
            return (roles: roles, selected: selectedRole);
        }

        [HttpGet("{id}")]
        [DisplayName("نقش با شناسه خاص")]
        // best is: Task<Role> Get([FromRoute]string id)
        public async Task<IActionResult> GetById([FromRoute]string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            if (!int.TryParse(id, out int Id))
            {
                return BadRequest();
            }
            var role = await _role.GetRoleById(Id);
            if (role == default)
            {
                return NotFound("موردی یافت نشد.");
            }
            return Ok(role);
        }
        [HttpGet("{id}")]
        [DisplayName("دریافت دسترسی های یک نقش خاص")]
        public async Task<List<Claim>> GetClaimByRoleId([FromRoute]string id)
        {
            if (!int.TryParse(id, out int Id))
            {
                return new List<Claim>();
            }
            return await _role.GetClaimsAsync(Id);

        }
        /// <summary>
        /// </summary>
        /// <params>
        /// </params>
        /// <returns>
        /// </returns>
        [HttpPost]
        [DisplayName("ثبت نقش")]
        public async Task<IActionResult> Post([FromBody](string rolename, List<Tuple<string, string>> claims) items)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            // http://hoctoantap.com/2018/08/27/implementing-resilient-applications.html
            //https://docs.microsoft.com/en-us/ef/ef6/fundamentals/connection-resiliency/retry-logic#limitations
            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;

            var strategy = _uow.strategy();
            if (items.rolename == adminUserSeed.RoleName)
            {
                return Content("این نقش در دسترس نیست.");
            }
            IActionResult result = BadRequest();
            await strategy.ExecuteAsync(async () =>
            {
                using (var tr = await _uow.tranAsync())
                {
                    try
                    {
                        var outputRole = await _role.CreateAsync(items.rolename);
                        if (outputRole == null)
                        {
                            // result = UnprocessableEntity();
                            result = BadRequest("خطا در ثبت نقش.");
                        }
                        else
                        {
                            // throw new Exception();
                            List<Claim> list = new List<Claim>();
                            if (items.claims.Any())
                            {
                                list = items.claims.Select(s =>
                                new Claim(s.Item1, s.Item2, ClaimValueTypes.String)).ToList();
                                var outClaims = await _role.AddClaimAsync(outputRole.Id/*.role*/, list);
                                if (outClaims.Succeeded)
                                {
                                    await _uow.SaveChangesAsync();
                                    tr.Commit();
                                    result = Ok(true);

                                }

                                else
                                    result = BadRequest(outClaims.Errors.FirstOrDefault().Description);

                            }
                            else
                            {
                                result = BadRequest("خطا برای نقش مورد نظر سطح دسترسی انتخاب نکردید.");
                            }
                        }

                    }
                    catch
                    {
                        tr.Rollback();
                        result = BadRequest("خطا سمت سرور");

                    }
                }
            });
            return result;
            #region Other Way
            // if EnableRetryOnFailure >> datalayer/context/DbContextOptionsExtensions.cs >>> methode :SetDbContextOptions>>> line 58
            // using (var tr = await _uow.tranAsync())

            // {
            //     try
            //     {
            //         var outputRole = await _role.CreateAsync(items.rolename);
            //       //  if (outputRole.exist == true)
            //         //{
            //          //   return Conflict();
            //         //}
            //         if (outputRole/*.role */== null /*&& outputRole.exist == true*/)
            //         {
            //           return UnprocessableEntity();
            //         }
            //         throw new Exception();
            //         if (items.claims.Any())
            //         {
            //             List<Claim> list = items.claims.Select(s =>
            //             new Claim(s.Item1, s.Item2, ClaimValueTypes.String)).ToList();
            //             await _role.AddClaimAsync(outputRole/*.role*/, list);
            //         }
            //         await _uow.SaveChangesAsync();
            //         tr.Commit();
            //     }
            //     catch(Exception ex)
            //     {
            //    // _uow.Rollback();
            //         tr.Rollback();
            //     }
            //}
            // return Ok(true);
            #endregion
        }

        [HttpPut]
        [DisplayName("ویرایش نقش")]
        public async Task<IActionResult> put([FromBody](Role role, List<Tuple<string, string>> claims) items)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
            if (items.role.Name == adminUserSeed.RoleName)
            {
                return NotFound("این نقش در دسترس نیست.");
            }
            var strategy = _uow.strategy();
            IActionResult result = BadRequest();
            await strategy.ExecuteAsync(async () =>
            {
                using (var tr = await _uow.tranAsync())
                {
                    try
                    {
                        var outputRole = await _role.EditAsync(items.role);

                        if (outputRole == default)
                        {
                            result = NotFound("نقش مورد نظر یافت نشد.");
                        }
                        else
                        {
                            //await _uow.SaveChangesAsync();

                            List<Claim> list = new List<Claim>();
                            if (items.claims.Any())
                            {
                                list = items.claims.Select(s =>
                                new Claim(s.Item1, s.Item2, ClaimValueTypes.String)).ToList();
                            }
                            var outClaims = await _role.AddClaimAsync((items.role).Id, list);

                            if (outClaims.Succeeded)
                            {

                                await _uow.SaveChangesAsync();
                                tr.Commit();
                                result = Ok(true);
                            }
                            else
                                result = BadRequest(outClaims.Errors.FirstOrDefault().Description);//NotFound(outClaims.Errors);

                        }

                    }
                    catch
                    {
                        tr.Rollback();
                        result = BadRequest("خطا سمت سرور");
                    }
                }
            });
            return result;
        }

        [HttpGet("{id:int}")]
        public List<RoleCustomeViewModel> GetRoleByUserId(int id)
        {
            var list = _role.GetRolesForUser(id);
            return list.Select(c => new RoleCustomeViewModel {Id= c.Id, Name= c.Name, Description= c.Description }).ToList();
        }
    #endregion
}
}