using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using shop.Services.Identity;
using shop.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace shop.Area.Api.Controllers
{
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantPolicies.ProductAdmin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class CategoryController : Controller
    {
        #region Fields
        private readonly ICategoryService _categoryService;
        private readonly IUploadService _uploadService;
        private IWebHostEnvironment _hostingEnvironment;
        #endregion

        #region Ctor
        public CategoryController(ICategoryService categoryService, 
        IUploadService uploadService,
         IWebHostEnvironment hostingEnvironment)
        {
            _categoryService = categoryService;
            _uploadService = uploadService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Methods    

        /// <summary>
        /// Looks up Service data by params.
        /// <returns>List services,TotalPage,Count</returns>
        /// </summary>
        [HttpGet]
        public async Task<PagingViewModel<CategoryViewModel>> Get(string title, int? id, int pageIndex, int pageSize, bool? isChkParent = false, bool? contionRoot=true)
        {
            var output = await _categoryService.Get(title: title, id: id, isChkParent: isChkParent, contionRoot: contionRoot , pageIndex: pageIndex, pageSize: pageSize);

            var response = new PagingViewModel<CategoryViewModel>
            {
                Pages = output.categories.Select(a => CategoryViewModel.FromEntity(a)).ToList(),
                TotalPage = output.totalPages,
                Count = output.count
            };

            return response;
        }

        [HttpGet]
        public async Task<PagingViewModel<CategoryViewModel>> GetDDown(string title, int? id, int pageIndex, int pageSize, bool? isChkParent = false, string order = null, bool? contionRoot=true)
        {
            var output = await _categoryService.Get(title: title, id: id, isChkParent: isChkParent, contionRoot: contionRoot, pageIndex: pageIndex, pageSize: pageSize);

            var response = new PagingViewModel<CategoryViewModel>
            {
                Pages = output.categories.Select(a =>new CategoryViewModel{
                    Id = a.Id,
                    Icon = a.Icon,
                    Order = a.Order,
                    ParentId = a.ParentId,
                    Path = a.Path,
                    Title = a.Title,
                }).ToList(),
                TotalPage = output.totalPages,
                Count = output.count
            };

            return response;
        }
        private static void ToFlat(List<CategoryTreeAppViewModel> flatList, Category item)
        {
            if (item == null)
            {
                return;
            }
            if (!flatList.Any(p => p.Id == item.Id))
            {
                flatList.Add(new CategoryTreeAppViewModel
                {
                    Icon = item.Icon == null ? "" : item.Id + "/" + item.Icon,
                    Id = item.Id,
                    ParentId = item.ParentId,
                    Title = item.Title
                });
            }

            if (item.ParentId != 1)
            {
                ToFlat(flatList, item.Parent);
            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("عدم اعتبار داده های وروردی");
            try
            {
                var filesIcon = new List<(string base64File, string filename, bool ValidSize)>();

                //// get filename
                if (model.Icon != default)
                {
                    filesIcon = await _uploadService.GetFileName(model.Icon, new List<string> { ".jpg", ".jpeg", ".png",".svg" }, 2048000);
                    if (filesIcon.FirstOrDefault().ValidSize != true)
                        return BadRequest($"حداکثر سایز مجاز برای ایکون KB 2000 است");

                    model.Icon = filesIcon.FirstOrDefault().filename;
                    model.Thumbnail = "thumbnail." + model.Icon;
                }

                //conflict
                var exist = await _categoryService.GetByTitle(title: model.Title);
                if (exist != null)
                    return Conflict("خطا در درج داده یکسان !");

                var output = await _categoryService.Create(model.ToEntity());

                if (!output.success)
                    return StatusCode((int)(output.status), new {message=output.error});

                if (model.Icon != null)
                {
                    var file = (filesIcon.FirstOrDefault().base64File, filesIcon.FirstOrDefault().filename);
                    await _uploadService.Upload(id: (output.category.Id).ToString(), file: file, uploadType: UploadType.Category);
                    await _uploadService.Upload(id: (output.category.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                    uploadType: UploadType.Category, width: 150, height: 150);
                }

                var OutModel = CategoryViewModel.FromEntity(output.category);
                // return Created("Create", OutModel);
                return Ok(true);


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new {message="خطا سمت سرور !"});

            }
        }

        [HttpPut("{id:int}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryViewModel model)
        {
            if (id != model.Id)
                return BadRequest("عدم اعتبار داده های وروردی");

            try
            {

                var filesIcon = new List<(string base64File, string filename, bool ValidSize)>();

                //// get filename
                if (model.Icon != default)
                {
                    filesIcon = await _uploadService.GetFileName(model.Icon, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 2048000);
                    if (filesIcon.FirstOrDefault().ValidSize != true)
                        return BadRequest( new {message=$"حداکثر سایز مجاز برای ایکون KB 2000 است"});

                    model.Icon = filesIcon.FirstOrDefault().filename;
                    model.Thumbnail = "thumbnail." + model.Icon;

                }

                //conflict
                var exist = await _categoryService.Get(title_phrase: model.Title, isChkParent: true);
                if (exist.count > 0)
                {
                    if (exist.categories.FirstOrDefault().Id != id)
                    {
                        return Conflict("خطا در درج داده یکسان !");
                    }
                }

                var output = await _categoryService.Update(id, model.ToEntity());

                if (!output.success)
                    return StatusCode((int)output.status ,new { message =output.error});


                if (model.Icon != null)
                {
                    // remove old file
                    if(!string.IsNullOrEmpty(output.oldFile))
                    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "category", id.ToString(), output.oldFile);
                    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "category", id.ToString(), "thumbnail." + output.oldFile);

                    //// upload file
                    var file = (filesIcon.FirstOrDefault().base64File, filesIcon.FirstOrDefault().filename);
                    await _uploadService.Upload(id: (output.category.Id).ToString(), file: file, uploadType: UploadType.Category);
                    await _uploadService.Upload(id: (output.category.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                    uploadType: UploadType.Category, width: 150, height: 150);
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new {message="خطا سمت سرور !"});
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                 var result = await _categoryService.Delete(id);
                 if (!result.success)
                     return StatusCode((int)result.statusCode, new {message=result.error});

                 return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new {message="خطا سمت سرور !"});
            }
        }

        #endregion
    }
}