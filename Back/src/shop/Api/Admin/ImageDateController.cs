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
using shop.Services.Identity;
using shop.ViewModels.Reservation;

namespace shop.Api.Admin.Controllers
{
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Admin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class ImageDateController : Controller
    {

        #region fiels
        private readonly IImageDateService _imageDateService;
        private IWebHostEnvironment _hostingEnvironment;

        private readonly IUploadService _uploadService;
        private readonly int _userId;
        private readonly IPictureService _pictureService;
        #endregion

        #region Ctor
        public ImageDateController(IImageDateService imageDateService,
           IHttpContextAccessor httpContextAccessor,
           IUploadService uploadService,
           IWebHostEnvironment hostingEnvironment,
           IPictureService pictureService)
        {
            _imageDateService = imageDateService;
            _uploadService = uploadService;
            _userId = httpContextAccessor.UserId();
            _hostingEnvironment = hostingEnvironment;
            _pictureService = pictureService;
        }
        #endregion

        #region Methods
        [HttpGet]
        public async Task<PagingViewModel<ImageDateViewModel>> Get(int? id, string date, int pageIndex = 1, int pageSize = 10)
        {
            var output = await _imageDateService.Get(id: id, date: date.ToGregorianDateTime(true), pageIndex, pageSize);

            var response = new PagingViewModel<ImageDateViewModel>
            {

                Pages = output.ImageDates.Select(a => ImageDateViewModel.FromEntity(a)).ToList(),
                Count = output.Count,
                TotalPage = output.TotalPages
            };
            return response;
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([FromForm] ImageDatePostViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های ورودی !");
            try
            {
                if (model.Images == null || !model.Images.Any())
                    return BadRequest("عکس الزامی است !");

                var imageDate = new ImageDate()
                {
                    Date = model.Date.ToGregorianDateTime(true).Value,
                    Images = new List<string>(),
                };
                #region validFile
                foreach (var item in model.Images)
                {
                    if (item != null && item.Length > 0)
                    {
                        var outValidFilePhoto = _uploadService.GetFileNameAndValidation(file: item,
                                       allowFormats: new List<string> { "image/png", "image/x-png", "image/jpeg", "image/jpg" }
                                       , 5000);

                        if (outValidFilePhoto.valid != true)
                            return BadRequest(outValidFilePhoto.error);

                        imageDate.Images.Add(outValidFilePhoto.filename);
                    }

                }
                #endregion
                var output = await _imageDateService.Create(imageDate);
                if (!output.success)
                    return StatusCode((int)output.status, output.error);

                #region UploadFile
                for (int i = 0; i < model.Images.Count; i++)
                {

                    if (model.Images[i] != null && model.Images[i].Length != 0)
                    {
                        await _uploadService.UploadImgAndResponsive(file: model.Images[i],
                        fileName: imageDate.Images[i],
                        path: $"/upload/ImageDate/{output.ImageDates.Id}",
                        maxPicWidth: 4000,
                        dimension: Dimensions.Width);
                    }
                }
                return Created("ImageDate", output.ImageDates);

                #endregion


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "خطا در ثبت !");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id, string date, string fileName=null)
        {
            if (id == null && date == null)
                return BadRequest("عدم اعتبار داده های وروردی !");

            var output = await _imageDateService.Delete(id, date.ToGregorianDateTime(true), fileName);
            if (!output.success)
                return StatusCode((int)output.status, output.error);

            if (output.oldFiles!= null && output.oldFiles.Any() && output.id >0 )
            {
                foreach (var item in output.oldFiles)
                  await _uploadService.RemoveFile($"{_hostingEnvironment.WebRootPath}/upload", "ImageDate", $"{output.id}",item);
            }
            return Ok(true);
        }
        #endregion
    }
}