using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Extension;
using shop.Services.Contracts;
using shop.Services.Contracts.Identity;
using shop.Services.Contracts.Reservation;
using shop.Services.Reservation;
using shop.ViewModels.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net;
using shop.Services.Identity;

namespace shop.Api.Admin {
    [Route ("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantRoles.Admin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class TagController : Controller {
        #region Fields
        private readonly ITagService _tagService;
        #endregion

        #region Ctor
        public TagController (ITagService tagService) {
            _tagService = tagService;
        }
        #endregion
        #region Methods
        [HttpGet]
        public async Task<PagingViewModel<TagViewModel>> Get (int? id = null, string title = null, int pageIndex = 0, int pageSize = 0, bool? option=false) {

            var output= await _tagService.Get(id:id , title: title, pageIndex: pageIndex, pageSize: pageSize ,
             includes: new Expression<Func<Tag, object>>[]{});

            var response = new PagingViewModel<TagViewModel> {
                Pages = output.tags.Select (a => TagViewModel.FromEntity (a)).ToList (),
                Count = output.count,
                TotalPage = output.totalPages,
             //   Provider = providerModel
            };
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] TagViewModel model) {
            if (!ModelState.IsValid)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {


                var output = await _tagService.Create (model.ToEntity());
                if (!output.success)
                   return StatusCode ((int) output.status, new { message = output.error});

                return Created ("tag", output.tag);
            } 
            catch (Exception ex) {
               return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }
        //update

        [HttpPut ("{id:int}")]
        public async Task<IActionResult> Update (int id, [FromBody] TagViewModel model) {
            if ( id != model.Id)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {

                var output = await _tagService.Update (model.ToEntity());
                if (!output.success)
                  return StatusCode ((int) output.status, new { message = output.error});

                return Ok (true);
            } 
            catch (Exception ex) {
               return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }

        [HttpDelete ("{id:int}")]
        public async Task<IActionResult> Delete (int id) {
            try {

                var output = await _tagService.Delete (id);
                if (!output.success)
                  return StatusCode ((int) output.status, new { message = output.error});
                    
                return Ok (true);
            } 
            catch (Exception ex) {
                return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }
        #endregion
    }
}