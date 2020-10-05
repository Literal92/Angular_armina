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

namespace shop.Api.Admin.Controllers {
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantPolicies.ProductAdmin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class ProductController : Controller {
        #region fields
        private readonly IProductService _productService;
        private IWebHostEnvironment _hostingEnvironment;

        private readonly IUploadService _uploadService;
        private readonly int _userId;
        private readonly IPictureService _pictureService;
        #endregion

        #region Ctor
        public ProductController (IProductService productService, 
            IHttpContextAccessor httpContextAccessor,
            IUploadService uploadService,
            IWebHostEnvironment hostingEnvironment, 
            IPictureService pictureService) {
            _productService = productService;
            _uploadService = uploadService;
            _userId = httpContextAccessor.UserId ();
            _hostingEnvironment=hostingEnvironment;
            _pictureService = pictureService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="categoryId"></param>
        /// <param name="withfield">شامل فیلد باشد یا نه؟</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagingViewModel<ProductViewModel>> Get (int? id, string title, string code, int? categoryId, bool? withfield, int pageIndex, int pageSize) {

            var inculde = new List<Expression<Func<Product, object>>>{ };
            if(withfield!= null && withfield == true)
                inculde.Add(c=>(c.ProductOptions as ProductOption).Field);

            if (id != null)
                inculde.Add(c => c.Pictures);

            var output = await _productService.Get (id:id, title: title, code: code, categoryId: categoryId, pageIndex: pageIndex, pageSize: pageSize,
                includes: inculde.ToArray());

            var response = new PagingViewModel<ProductViewModel> {
                Pages = output.Products.Select (a => ProductViewModel.FromEntity (a)).ToList (),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }

        [HttpPost]
       [DisableRequestSizeLimit]
        public async Task<IActionResult> Create ([FromBody] ProductViewModel model) {
            if (!ModelState.IsValid)
                return BadRequest ("عدم اعتبار داده های ورودی !");

            try {
                var files = new List < (string base64File, string filename, bool ValidSize) > ();
                var galleryFiles = new List<(string base64File, string filename, bool ValidSize)>();

                var gallery = new List<(string pic, string thumbnail)>();
                //// get filename
                if (model.Pic != default) {
                    files = await _uploadService.GetFileName (model.Pic, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 2048000);
                    if (files.FirstOrDefault ().ValidSize != true)
                        return BadRequest (new { message = $"حداکثر سایز مجاز برای ایکون KB 2000 است" });

                    model.Pic = files.FirstOrDefault ().filename;
                    model.Thumbnail = "thumbnail."+ model.Pic;
                }

                if (model.Gallery != null && model.Gallery.Any())
                {
                    foreach (var item in model.Gallery)
                    {
                        var valid = await _uploadService.GetFileName(item, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 2048000);
                        if (valid.FirstOrDefault().ValidSize != true)
                            return BadRequest(new { message = $"حداکثر سایز مجاز برای عکس گالری KB 2000 است" });

                        galleryFiles.AddRange(valid);

                        var pic = valid.FirstOrDefault().filename;
                        var thumbnail = "thumbnail." + pic;

                        gallery.Add((pic, thumbnail));

                    }
                }

                var output = await _productService.Create (model.ToEntity ());

                if (!output.success)
                    return StatusCode ((int) output.status, new { message = output.error});


                foreach (var item in gallery)
                {
                    var picture = new Picture();
                    picture.Photo = item.pic;
                    picture.Thumbnail = item.thumbnail;
                    picture.ProductId = output.product.Id;
                   _= await _pictureService.Create(picture);
                }

                if (!string.IsNullOrEmpty (model.Pic)) {
                    var file = (files.FirstOrDefault ().base64File, files.FirstOrDefault ().filename);
                    await _uploadService.Upload (id: (output.product.Id).ToString (), file : file, uploadType : UploadType.Product);
                    await _uploadService.Upload(id: (output.product.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                   uploadType: UploadType.Product, width: 450);
                }
                if (galleryFiles != null && galleryFiles.Any())
                {
                    foreach (var item in galleryFiles)
                    {
                        var file = (item.base64File, item.filename);
                        await _uploadService.Upload(id: (output.product.Id).ToString(), file: file, uploadType: UploadType.Product);
                        await _uploadService.Upload(id: (output.product.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                        uploadType: UploadType.Product, width: 450);
                    }
                }
                return Ok (true);

            }
            catch (Exception ex) {
                return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }

        [HttpPut ("{id:int}")]
       [DisableRequestSizeLimit]

        public async Task<IActionResult> Update (int id, [FromBody] ProductViewModel model) {
            if (id != model.Id)
                return BadRequest ("عدم اعتبار داده های ورودی !");
            try {
                #region validation
                var files = new List <(string base64File, string filename, bool ValidSize) > ();
                var galleryFiles = new List<(string base64File, string filename, bool ValidSize)>();

                var gallery = new List<(string pic,string thumbnail)>();
                //// get filename
                if (model.Pic != default) {
                    files = await _uploadService.GetFileName (model.Pic, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 2048000);
                    if (files.FirstOrDefault ().ValidSize != true)
                        return BadRequest (new { message = $"حداکثر سایز مجاز برای ایکون KB 2000 است" });

                    model.Pic = files.FirstOrDefault ().filename;
                    model.Thumbnail = "thumbnail." + model.Pic;
                }

                if(model.Gallery!= null && model.Gallery.Any())
                {
                    foreach (var item in model.Gallery)
                    {
                        var valid = await _uploadService.GetFileName(item, new List<string> { ".jpg", ".jpeg", ".png", ".svg" }, 2048000);
                        if (valid.FirstOrDefault().ValidSize != true)
                            return BadRequest(new { message = $"حداکثر سایز مجاز برای عکس گالری KB 2000 است" });

                        galleryFiles.AddRange(valid);

                        var pic = valid.FirstOrDefault().filename;
                        var thumbnail = "thumbnail." + pic;
                       
                        gallery.Add((pic, thumbnail));

                    }
                }
                #endregion

                var product = await _productService.GetById (id: id);

                if (product == null)
                    return NotFound (new { message = "ایتمی یافت نشد !" });

                var productToUpdate= model.ToEntity();

                productToUpdate.Pic = !string.IsNullOrEmpty (model.Pic) ? model.Pic : product.Pic;
                productToUpdate.Thumbnail = !string.IsNullOrEmpty(model.Pic) ? model.Thumbnail : product.Thumbnail;

                var output = await _productService.Update(productToUpdate);

                if (!output.success)
                    return StatusCode((int)(output.status), new {message=output.error});

                foreach (var item in gallery)
                {
                    var picture = new Picture();
                    picture.Photo = item.pic;
                    picture.Thumbnail = item.thumbnail;
                    picture.ProductId = output.product.Id;
                   _= await _pictureService.Create(picture);
                }


                //if (!string.IsNullOrEmpty (model.Pic)) {

                //  // remove old file
                //    if(!string.IsNullOrEmpty(output.oldFile))
                //    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", id.ToString(), output.oldFile);
                //    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", id.ToString(),
                //        "thumbnail." + output.oldFile);

                //    // upload new file
                //    var file = (files.FirstOrDefault ().base64File, files.FirstOrDefault ().filename);
                //    await _uploadService.Upload (id: (output.product.Id).ToString (), file : file, uploadType : UploadType.Product);
                //    await _uploadService.Upload(id: (output.product.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                //   uploadType: UploadType.Product, width: 450);
                //}
                if (!string.IsNullOrEmpty(model.Pic))
                {
                    // remove old file
                    if (!string.IsNullOrEmpty(output.oldFile))
                        await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", id.ToString(), output.oldFile);
                    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", id.ToString(),
                        "thumbnail." + output.oldFile);

                    // upload new file
                    var file = (files.FirstOrDefault().base64File, files.FirstOrDefault().filename);
                    await _uploadService.Upload(id: (output.product.Id).ToString(), file: file, uploadType: UploadType.Product);
                    await _uploadService.Upload(id: (output.product.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                   uploadType: UploadType.Product, width: 450);
                }
                if(galleryFiles != null && galleryFiles.Any())
                {
                    foreach (var item in galleryFiles)
                    {
                         var file = (item.base64File, item.filename);
                         await _uploadService.Upload(id: (output.product.Id).ToString(), file: file, uploadType: UploadType.Product);
                         await _uploadService.Upload(id: (output.product.Id).ToString(), file: (file.base64File, "thumbnail." + file.filename),
                         uploadType: UploadType.Product, width: 450);
                    }
                }
                return Ok(true);
            } 
            catch (Exception ex) {
                return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }

        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                 var result = await _productService.Delete(id);

                 if (!result.success)
                     return StatusCode((int)result.status, new {message= result.error});
                                
                    return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new {message="خطا سمت سرور !"});
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePic(int id)
        {
            try
            {
                var result = await _pictureService.Delete(id);

                if (!result.success)
                    return StatusCode((int)result.status, new { message = result.error });

                if (!string.IsNullOrEmpty(result.oldFile))
                {
                    // remove old file
                    if (!string.IsNullOrEmpty(result.oldFile))
                        await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", result.pId.ToString(), result.oldFile);
                    await _uploadService.RemoveFile(_hostingEnvironment.WebRootPath + "\\upload", "product", result.pId.ToString(),
                        result.oldThumbnail);
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }


        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ActiveProduct(int id)
        {
            var output = await _productService.UpdateActive(id);
            if (!output.success)
                return StatusCode((int)output.status, output.error);

            return Ok(true);
        }
        #endregion
    }
}