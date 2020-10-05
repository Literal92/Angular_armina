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
    [Authorize(Policy = ConstantRoles.ProductAdmin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class FieldController : Controller {
        #region Fields
        private readonly IFieldService _fieldService;
        #endregion

        #region Ctor
        public FieldController (IFieldService fieldService) {
            _fieldService = fieldService;
        }
        #endregion
        #region Methods
        [HttpGet]
        public async Task<PagingViewModel<FieldViewModel>> Get (int? id = null, string title = null, int pageIndex = 0, int pageSize = 0, bool? option=false) {

            var output= await _fieldService.Get(id:id , title: title, pageIndex: pageIndex, pageSize: pageSize ,
             includes: new Expression<Func<Field, object>>[]{});

            var response = new PagingViewModel<FieldViewModel> {
                Pages = output.fields.Select (a => FieldViewModel.FromEntity (a)).ToList (),
                Count = output.count,
                TotalPage = output.totalPages,
             //   Provider = providerModel
            };
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] FieldViewModel model) {
            if (!ModelState.IsValid)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {


                var output = await _fieldService.Create (model.ToEntity ());
                if (!output.success)
                   return StatusCode ((int) output.status, new { message = output.error});

                return Created ("filed", output.field);
            } 
            catch (Exception ex) {
               return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }
        //update

        [HttpPut ("{id:int}")]
        public async Task<IActionResult> Update (int id, [FromBody] FieldViewModel model) {
            if ( id != model.Id)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {

                var output = await _fieldService.Update (model.ToEntity());
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

                var output = await _fieldService.Delete (id);
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