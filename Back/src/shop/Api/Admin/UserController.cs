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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using shop.ViewModels.Reservation;
using shop.Services.Contracts.Reservation;
using shop.Entities.Reservation.Enum;
using System.Net;
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
    //[ApiExplorerSettings(IgnoreApi = true)]


    //[DisplayName("کارمندان")]
    public class UserController : Controller //ControllerBase
    {
        #region Fields
        private readonly IApplicationUserManager _user;
        private readonly IUnitOfWork _uow;
        private readonly IOptionsSnapshot<SiteSettings> _adminUserSeedOptions;
        private readonly IUploadService _uploadService;

        #endregion

        #region Ctor

        public UserController(IApplicationUserManager user,
            IUnitOfWork uow,
            IOptionsSnapshot<SiteSettings> adminUserSeedOptions,
            IUploadService uploadService,

            IApplicationUserManager _applicationUserManager)
        {
            _user = user;
            _uow = uow;
            _adminUserSeedOptions = adminUserSeedOptions;
            _uploadService = uploadService;

        }
        #endregion

        #region Methods     
        [HttpGet]
        [DisplayName("نمایش کارمندان")]
        public async Task<(IEnumerable<User> users, int count, int totalPage)> Get(int? id, int pageSize, int pageIndex, string Mobile, string displayName, int? status)
        {
            displayName = displayName != "undefined" ? displayName : null;
            Mobile = Mobile != "undefined" ? Mobile : null;
            status = status == -1 ? null : status;
            var users = await _user.Get(id: id, pageIndex: pageIndex, pageSize: pageSize, displayName: displayName, status: status, Mobile: Mobile);
            var Users = users.Users;
            var count = users.Count;
            var totalpage = users.TotalPages;

            return (users: Users, count: count, totalPage: totalpage);
        }
        [HttpGet("{id}")]
        // [UserClaimRequirementFilter(${id})]
        [DisplayName("کاربر با شناسه خاص")]
        //best is:  Task<User> GetById([FromRoute]string id)
        public async Task<IActionResult> GetById([FromRoute]string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            //if (!int.TryParse(id, out int Id))
            //{
            //    return BadRequest();
            //}
            var user = await _user.FindByIdAsync(id);
            if (user == default)
            {
                return NotFound("موردی یافت نشد.");
            }
            return Ok(user);
        }

        [HttpPost]
        [DisplayName("ثبت کاربر")]
        //public async Task<IActionResult> Post([FromBody](User user, string password, List<Role> roles) items)
        public async Task<IActionResult> Post([FromBody](User user, string password, List<Role> roles/*, List<Tuple<string, string>> claims*/) items)

        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");


            // http://hoctoantap.com/2018/08/27/implementing-resilient-applications.html
            //https://docs.microsoft.com/en-us/ef/ef6/fundamentals/connection-resiliency/retry-logic#limitations
            var strategy = _uow.strategy();
            IActionResult result = BadRequest();
            await strategy.ExecuteAsync(async () =>
            {
                using (var tr = await _uow.tranAsync())
                {
                    try
                    {
                        // add user
                        var outputUser = await _user.CreateAsync(items.user, items.password);
                        if (!outputUser.Succeeded)
                        {
                            result = BadRequest(outputUser.Errors.FirstOrDefault().Description);
                        }
                        else
                        {
                            // await _uow.SaveChangesAsync();
                            items.user.Id = (await _user.FindByNameAsync(items.user.UserName)).Id;
                            // add UserRole
                            if (items.roles.Any())
                            {
                                var outUserRole = await _user.AddUserRoleAsync(items.user.Id, items.roles);
                            }
                            // add userClaims
                            //if (items.claims.Any())
                            //{
                            //    List<Claim> list = items.claims.Select(s =>
                            //    new Claim(s.Item1, s.Item2, ClaimValueTypes.String)).ToList();
                            //    await _user.AddClaimAsync(items.user,list);
                            //}

                            await _uow.SaveChangesAsync();
                            tr.Commit();
                            result = Ok(true);
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

        [HttpPut]
        [DisplayName("ویرایش کاربر")]
        public async Task<IActionResult> Update([FromBody] UpdateUserInfoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("عدم اعتبار داده های وروردی");

                var user = await _user.FindByIdAsync(model.Id.ToString());
                user.UserName = model.UserName;
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.natioalCode = model.NatioalCode;
                user.Mobile = model.Mobile;
                user.PhoneNumber = model.PhoneNumber;

                var outputUser = await _user.UpdateUserAsync(user, null);

                if (!outputUser.Succeeded)
                    return BadRequest(outputUser.Errors.FirstOrDefault().Description);

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPut("{id}")]
        [DisplayName("ویرایش نقش کاربر")]
        public async Task<IActionResult> PutRole([FromRoute]int id, [FromBody]List<RoleCustomeViewModel> model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            try
            {
                var user = await _user.FindByIdAsync(id.ToString());
                if (user == default)
                    return NotFound("کاربر یافت نشد.");
                var roles = model.Select(c => new Role { Id = c.Id, Name = c.Name, Description = c.Description }).ToList();
                var outUserRole = await _user.AddUserRoleAsync(id, roles);

                if (!outUserRole.Succeeded)
                {
                    var errors = outUserRole.Errors.ToList();
                    var error = errors[0].Description;
                    return BadRequest(error);
                }
                await _uow.SaveChangesAsync();
                return Ok(true);
            }
            catch
            {
                return BadRequest("خطا سمت سرور");
            }
        }



        [HttpPost]
        [DisplayName("ثبت مجوز برای هر کاربر")]
        public async Task<IActionResult> Claims([FromBody](int id, List<UserClaim> claims) Inputs)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            try
            {
                var user = await _user.FindByIdAsync((Inputs.id).ToString());
                if (user == null)
                {
                    return NotFound("کاربر یافت نشد.");
                }
                await _user.AddUserClaimsAsync(user, Inputs.claims);
                await _uow.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("خطا سمت سرور");
            }
            return Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass([FromBody](int id, string password) items)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");

            try
            {
                //warning: dont forget check conflict()
                var strategy = _uow.strategy();
                var user = await _user.FindByIdAsync((items.id).ToString());
                if (user == null)
                {
                    return NotFound("کاربر یافت نشد.");
                }
                var outputUser = await _user.ChangePasswordAsync(user, items.password);
                if (outputUser == default)
                {
                    return NotFound("کاربر یافت نشد.");
                }
                await _uow.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("خطا سمت سرور");
            }

            return Ok(true);
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ActiveUser(int id)
        {
            var output = await _user.UpdateActive(id);
            if (!output.success)
                return StatusCode((int)output.status, output.error);

            return Ok(true);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ChangetypeUser(int id)
        {
            var output = await _user.ChangeTypeUser(id);
            if (!output.success)
                return StatusCode((int)output.status, output.error);

            return Ok(true);
        }

        [HttpPost]
        [DisplayName("جستجو کارمندان")]
        public async Task<IActionResult> Search([FromBody] dynamic data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("عدم اعتبار داده های وروردی");

                string phrase = data.phrase != null ? (data.phrase.Value).ToLower() : null;
                var adminUserSeed = _adminUserSeedOptions.Value.AdminUserSeed;
                var users = await _user.GetUserAsync(
                     where: (c => ((c.UserName).ToLower() != (adminUserSeed.Username).ToLower())
                                     &&
                                  (
                                  (EF.Functions.Like((c.DisplayName).ToLower(), "%" + phrase + "%") ||
                                    EF.Functions.Like((c.LastName).ToLower(), "%" + phrase + "%") ||
                                    EF.Functions.Like((c.UserName).ToLower(), "%" + phrase + "%") ||
                                    EF.Functions.Like((c.Mobile).ToLower(), "%" + phrase + "%") ||
                                    EF.Functions.Like(c.PhoneNumber, "%" + phrase + "%")))
                     ),
                 includeProperties: c => c.Claims);
                return Ok(users);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserPostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "عدم اعتبار داده های ورودی !" });
            try
            {
               
                

                var user = new User
                {
                    UserName = model.UserName,

                    Mobile = model.UserName,
                    PhoneNumber = model.UserName,
                   // CityId = model.CityId == 0 ? null : model.CityId,
                    FirstName = model.FirstName,
                    //LastName = model.LastName,
                    //natioalCode = model.NatioalCode,
                   // PhotoFileName = model.PhotoFileName,
                    IsActive = true

                };

                var output = await _user.CreateWithRoleAsync(user, model.Password, "client");
                if (!output.result.Succeeded)
                    return BadRequest(new { message = output.result.Errors.FirstOrDefault() });

              

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا در ثبت !" });
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<UserSimpleViewModel>> GetSimple()
        {
            var output = await _user.Get();
            var Users = output.Users;
            var response = new List<UserSimpleViewModel>();
            foreach (var item in Users)
            {
                response.Add(new UserSimpleViewModel { Id = item.Id, DisplayName = item.DisplayName ?? item.UserName  });
            }
            return response;
        }

       
        #endregion
    }
}
