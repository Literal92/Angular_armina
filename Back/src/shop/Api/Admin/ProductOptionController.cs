using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net;
using shop.Services.Contracts.Reservation;
using shop.ViewModels.Reservation;
using shop.Entities.Reservation;
using shop.Services.Identity;
using shop.Services.Reservation;

namespace quiz.Api.Admin {
    [Route("api/Admin/[controller]/[action]")]
    [Authorize(Policy = ConstantPolicies.ProductAdmin)]
    [ApiExplorerSettings(GroupName = "v2.0")]

    public class ProductOptionController : Controller {
        #region Fields
        private readonly IProductOptionService _productOptionService;
        private readonly IOptionColorService _optionColorService;

        #endregion

        #region Ctor
        public ProductOptionController (IProductOptionService productOptionService, IOptionColorService optionColorService) {
            _productOptionService = productOptionService;
            _optionColorService = optionColorService;
        }

        #endregion
        #region Methods
        [HttpGet]
        public async Task<PagingViewModel<ProductOptionViewModel>> Get (int? id = null,int? fieldId= null, int? productId =null,
         int pageIndex = 0, int pageSize = 0) {

             var include= new List<Expression<Func<ProductOption, object>>>{c=> c.Field, c=>c.OptionColors};

             if(id != null) 
             include.Add(c=>c.Product);

            var output= await _productOptionService.Get(id:id,fieldId: fieldId, productId: productId, pageIndex: pageIndex, pageSize: pageSize ,
             includes: include.ToArray() );

            var response = new PagingViewModel<ProductOptionViewModel> {
                Pages = output.options.Select (a => ProductOptionViewModel.FromEntity(a)).ToList(),
                Count = output.Count,
                TotalPage = output.TotalPages,
             //   Provider = providerModel
            };
            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] ProductOptionViewModel model) {
            
            if (!ModelState.IsValid)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {


                var output = await _productOptionService.Create (model.ToEntity ());
                if (!output.success)
                   return StatusCode ((int) output.status, new { message = output.error});

                return Created ("productOption", output.option);
            } 
            catch (Exception ex) {
               return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });

            }
        }

        [HttpPut ("{id:int}")]
        public async Task<IActionResult> Update (int id, [FromBody] ProductOptionViewModel model) {
            if ( id != model.Id)
                return BadRequest ("عدم اعتبار داده های وروردی");
            try {

                var output = await _productOptionService.Update(model.ToEntity());
                if (!output.success)
                  return StatusCode ((int) output.status, new { message = output.error});

                return Ok (true);
            } 
            catch (Exception ex) {
               return StatusCode ((int) HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateColor(int id, [FromBody] OptionColorViewModel model)
        {
            if (id != model.Id)
                return BadRequest("عدم اعتبار داده های وروردی");
            try
            {

                var output = await _optionColorService.Update(model.ToEntity());
                if (!output.success)
                    return StatusCode((int)output.status, new { message = output.error });

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "خطا سمت سرور !" });
            }
        }

        [HttpDelete ("{id:int}")]
        public async Task<IActionResult> Delete (int id) {
            try {

                var output = await _productOptionService.Remove(id);
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