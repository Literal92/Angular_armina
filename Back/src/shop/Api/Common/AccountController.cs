using System.Security.Claims;
using System.Threading.Tasks;
using shop.DataLayer.Context;
using shop.Services.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using shop.Common.GuardToolkit;
using shop.Services.Contracts.Identity;
using Microsoft.Extensions.Options;
using shop.ViewModels.Identity.Settings;
using shop.Entities.Identity;
using System;
using shop.ViewModels.Reservation;
using shop.Services.Contracts.Reservation;
using shop.Entities.Reservation.Enum;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq.Expressions;
using shop.Extension;

using shop.Common.Sms;
using DNTPersianUtils.Core;

namespace shop.Controllers
{
    [Route("api/Common/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v3.0")]

    public class AccountController : Controller
    {
        private IApplicationUserManager _usersService;
        private IUnitOfWork _uow;
        //private IAntiForgeryCookieService _antiforgery;
        private ITokenStoreService _tokenStoreService;
        private ITokenFactoryService _tokenFactoryService;
        private readonly IUploadService _uploadService;
        private readonly IOptionsSnapshot<SiteSettings> _settingOptions;
        private readonly IApplicationRoleManager _role;
        private readonly User _user;
        private readonly IApplicationSignInManager _signinManager;
        public AccountController(
            IApplicationRoleManager role,
            IApplicationUserManager usersService,
            IUnitOfWork uow,
            //IAntiForgeryCookieService antiforgery,
            ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService,
            IOptionsSnapshot<SiteSettings> settingOptions,
            IApplicationSignInManager applicationSignInManager,
            IApplicationSignInManager signinManager,
            IUploadService uploadService,
            IHttpContextAccessor httpContextAccessor
            )
        {

            _settingOptions = settingOptions;
            _role = role;

            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _tokenStoreService = tokenStoreService;
            _tokenStoreService.CheckArgumentIsNull(nameof(tokenStoreService));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            //_antiforgery = antiforgery;
            //_antiforgery.CheckArgumentIsNull(nameof(antiforgery));

            _tokenFactoryService = tokenFactoryService;
            _tokenFactoryService.CheckArgumentIsNull(nameof(tokenFactoryService));

            _signinManager = signinManager;

            _uploadService = uploadService;
            _user = httpContextAccessor.User(_usersService);
        }

        [HttpPost]
        [AllowAnonymous]
        //[IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login([FromBody](string username, string password, string captcha) items)
        {

            if (!ModelState.IsValid)
                return BadRequest("user is not set.");

            var user = await _usersService.FindUserAsync2(items.username, items.password);
            if (user == null)
                return NotFound("نام کاربری یا رمز عبور اشتباه است.");

            if (user.Roles == null || !user.Roles.Any())
                return BadRequest("مجوز دسترسی برای شما صادر نگردیده است.");

            var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
            await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
            await _uow.SaveChangesAsync();

            //_antiforgery.RegenerateAntiForgeryCookies(result.Claims);

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody]JToken jsonBody)
        {
            var refreshTokenValue = jsonBody.Value<string>("refreshToken");
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return BadRequest("refreshToken is not set.");
            }

            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
            {
                return Unauthorized();
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(token.User);
            await _tokenStoreService.AddUserTokenAsync(token.User, result.RefreshTokenSerial, result.AccessToken, _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));
            await _uow.SaveChangesAsync();

            //_antiforgery.RegenerateAntiForgeryCookies(result.Claims);

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<bool> Logout(string refreshToken)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            //var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;
            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber)?.Value;
            var userIdValue = (await _usersService.FindBySerialNumber(serialNumberClaim))?.Id;

            // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
            // Delete the user's tokens from the database (revoke its bearer token)
            await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue.ToString(), refreshToken);
            await _uow.SaveChangesAsync();

            //_antiforgery.DeleteAntiForgeryCookies();

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody](int userId, string password, int verifyCode) items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var _user = _usersService.FindById(items.userId);
            if (_user == null)
            {
                return NotFound("کاربری پیدا نشد.");
            }
            if (_user.VerifyCode != items.verifyCode)
                return NotFound("کاربری پیدا نشد.");

            if (_user.IsRequestRestPassword != true)
                return NotFound("کاربری پیدا نشد.");

            var output = await _usersService.ChangePasswordAsync(_user, items.password);
            if (output == null)
            {
                return UnprocessableEntity();
            }
            _user.IsRequestRestPassword = false;
            var res = await _usersService.UpdateAsync(_user);
            if (!res.Succeeded)
                return BadRequest("خطا در زمان تغییر رمز عبور، لطفا مجددا تلاش نمایید.");

            return Ok(true);
        }

        [HttpGet, HttpPost]
        public bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet, HttpPost]
        public IActionResult GetUserInfo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return Json(new { Username = claimsIdentity.Name });
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> GetToken(string number)
        {
            //number = "0" + number;

            if (number.IsValidIranianMobileNumber())
            {
                var user = await _usersService.GetVerifyCode(number.ToEnglishNumbers());

                //Todo:  ارسال sms به کاربر
                var api = new Kavenegar.KavenegarApi("41723573576F786B63616A7263594F6346734D584E52395036536A7836456279");
                var res = await api.VerifyLookup(number, user.VerifyCode.ToString(), "activation");
            }
            else
            {
                return BadRequest(new { message = "شماره موبایل ارسال شده معتبر نیست" });
            }

         



            return Ok(new { message = "رمز عبور یکبار مصرف برای شما ارسال شد." });
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> ForgotPassword(string number)
        {
            var user = _usersService.FindByUsername(number);
            var rand = new Random();
            var code = rand.Next(1000, 9999);
            code = 1234;
            if (user == null)
                return BadRequest("کاربری با این شماره همراه پیدا نشد.");

            if (user.IsRegister != true)
                return BadRequest("کاربری با این شماره همراه پیدا نشد.");

            user.VerifyCode = code;
            user.ExpireDateVerifyCode = DateTime.Now.AddDays(1);
            var result = await _usersService.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            ////Todo:  ارسال sms به کاربر
            //var api = new Kavenegar.KavenegarApi("kavenegarID");
            //var res = api.VerifyLookup(number, code.ToString(), "activation");
            // SmsService.SendSms(number, "با سلام رمز یکبار مصرف شما " + code);

            return Ok(new { message = "رمز عبور یکبار مصرف برای شما ارسال شد." });
        }

        [HttpGet("{username}/{token:int}")]
        public async Task<IActionResult> CheckToken(string username, string token)
        {

            token = token.ToEnglishNumbers();
            username = username.ToEnglishNumbers();
            if (!username.IsValidIranianMobileNumber())
                return BadRequest(new { message = "شماره همراه وارد شده معتبر نمیباشد" });

            var user = _usersService.FindByUsername(username);
            if (user == null)
                return BadRequest(new { message = "کاربری با این شماره پیدا نشد. لطفا مجددا ثبت نام نمایید." });
            if (user.ExpireDateVerifyCode < DateTime.Now)
                return BadRequest(new { message = "اعتبار رمز یکبارمصرف شما به اتمام رسیده است. لطفا مجددا ثبت نام نمایید." });
            if (user.VerifyCode != Convert.ToInt32(token))
                return BadRequest(new { message = "رمز وارد شده صحیح نمی باشد." });

            //???
            user.VerifyCode = null;
            user.ExpireDateVerifyCode = null;



            var result = await _usersService.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);


            //if (user.Roles == null || !user.Roles.Any()) 
            //    return BadRequest("مجوز دسترسی برای شما صادر نگردیده است.");

            var result1 = await _tokenFactoryService.CreateJwtTokensAsync(user);
            await _tokenStoreService.AddUserTokenAsync(user, result1.RefreshTokenSerial, result1.AccessToken, null);
            await _uow.SaveChangesAsync();

            return Ok(new { access_token = result1.AccessToken, refresh_token = result1.RefreshToken });
        }

        [HttpPost("{id:int}")]
        public async Task<IActionResult> Register(int id, [FromBody]RegisterUser data)
        {
            var user = _usersService.FindById(id);
            if (user == null)
                return BadRequest("کاربری با این شماره پیدا نشد. لطفا مجددا ثبت نام نمایید.");
            if (user.IsRegister == true)
                return BadRequest("کاربر مورد نظر قبلا ثبت نام نموده است.");
            //خواندن جدول تنظیمات
            // var setting = await _settingService.Get();

            user.FirstName = data.FirstName;
            user.LastName = data.LastName;
            user.IsRegister = true;
            user.IsActive = true;

            if (data.RefererCode != null && data.RefererCode.Length > 0)
            {
                var refererUser = _usersService.FindByUsername(username: data.RefererCode);
                if (refererUser == null)
                    return BadRequest("کد معرف وارد شده نامعتبر است.");

            }


            var r = await _usersService.UpdateUserAsync(user, data.Password);
            if (!r.Succeeded)
                return BadRequest(r.Errors.FirstOrDefault() != null ? r.Errors.FirstOrDefault().Description : "خطا در ارتباط با سرور");


            if (!data.IsProvider)
            {
                var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
                await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
                await _uow.SaveChangesAsync();

                //_antiforgery.RegenerateAntiForgeryCookies(result.Claims);

                return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
            }
            else
            {
                return Ok(new { message = "ثبت نام با موفقیت انجام شد.", value = new { id = user.Id } });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            _user.WalletPrice = _user.WalletPrice ?? 0;
            _user.Score = _user.Score ?? 0;
            // _user.AreaId = _user.AreaId ?? 0;
            // _user.StateId = _user.StateId ?? 0;
            // _user.CityId = _user.CityId ?? 0;
            return Ok(_user);
        }

        [HttpPost]
        public async Task<IActionResult> ActiveUser([FromBody] UpdateUserViewModel data)
        {
            User user = new User
            {
                Id = data.Id,
                IsActive = data.IsActive.Value
            };
            var result = await _usersService.UpdateUserAsync(user, null);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors.Any() ? result.Errors.FirstOrDefault().Description : "خطا در زمان ذخیره تغییرات" });
            return Ok(new { message = "تغییرات با موفقیت اعمال شد." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel data)
        {

            _user.FirstName = data.FirstName;
            _user.LastName = data.LastName;
            //_user.StateId = data.StateId;
            //_user.AreaId = data.AreaId;
            //_user.CityId = data.CityId;
            _user.IsActive = data.IsActive != null ? data.IsActive.Value : _user.IsActive;

            if (data.Password != null && data.Password != "" && data.Password.Length > 0)
            {
                var result = await _usersService.UpdateUserAsync(_user, data.Password);
                if (!result.Succeeded)
                    return BadRequest(new { message = result.Errors.Any() ? result.Errors.FirstOrDefault().Description : "خطا در زمان ذخیره تغییرات" });
            }
            else
            {
                var result = await _usersService.UpdateAsync(_user);
                if (!result.Succeeded)
                    return BadRequest(new { message = result.Errors.Any() ? result.Errors.FirstOrDefault().Description : "خطا در زمان ذخیره تغییرات" });
            }

            //var res = await _tokenFactoryService.CreateJwtTokensAsync(_user);
            //await _tokenStoreService.AddUserTokenAsync(_user, res.RefreshTokenSerial, res.AccessToken, null);
            //await _uow.SaveChangesAsync();

            //_antiforgery.RegenerateAntiForgeryCookies(res.Claims);

            return Ok(new { message = "تغییرات با موفقیت اعمال شد." });
        }
    }
}