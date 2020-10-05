using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Extension;
using shop.Services.Contracts;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.ViewModels.Reservation;

namespace shop.Api.Client
{
    [Route("api/client/[controller]/[action]")]

    public class CooperationController : Controller
    {
        #region fields
        private readonly IApplicationUserManager _applicationUserManager;
        private readonly IUploadService _uploadService;
        #endregion

        #region Ctor
        public CooperationController(IApplicationUserManager applicationUserManager, IUploadService uploadService)
        {
            _applicationUserManager = applicationUserManager;
            _uploadService = uploadService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> SaveCooperation([FromForm] CooperationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی");

            try
            {
                if (!model.Mobile.IsValidIranianMobileNumber())
                    return BadRequest("شماره موبایل ارسال شده معتبر نیست");

                var user = new User
                {
                    IsRequest = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mobile = model.Mobile.ToEnglishNumbers(),
                    UserName = model.Mobile.ToEnglishNumbers(),
                    Address = model.Address,
                    WhatsApp = model.WhatsApp,
                    Telegram = model.Telegram,
                    Instagram = model.Instagram,
                    WebSite = model.WebSite
                };

                #region validFile
                if (model.Picture != null && model.Picture.Length > 0)
                {
                    var outValidFile = _uploadService.GetFileNameAndValidation(file: model.Picture,
                                       allowFormats: new List<string> { "image/png", "image/x-png", "image/jpeg", "image/jpg" }
                                       , 10000);

                    if (outValidFile.valid != true)
                        return BadRequest(outValidFile.error);

                    user.Picture = outValidFile.filename;
                }

                #endregion

                var output = await _applicationUserManager.CooperationRequest(user);
                if (output == null)
                throw new InvalidOperationException("خطا در ثبت در خواست");

                #region UploadFile

                  else if (model.Picture != null && model.Picture.Length > 0)
                {
                    await _uploadService.UploadImgAndResponsive(file: model.Picture,
                    fileName: user.Picture,
                    path: $"/upload/user/{output.Id}",
                    maxPicWidth: 10000,
                    dimension: Dimensions.Width);
                }
                #endregion

                return Created("user", true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا در ثبت !");
            }

        }

        #endregion
    }
}