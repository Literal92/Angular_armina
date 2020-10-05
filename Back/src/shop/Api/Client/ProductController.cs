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
    [ApiExplorerSettings(GroupName = "v1.0")]

    public class ProductController : Controller
    {
        #region fields
        private readonly IProductService _productService;
        private readonly int _userId;

        #endregion

        #region Ctor
        public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _userId = httpContextAccessor.UserId();
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
        [HttpGet]
        public async Task<PagingViewModel<ProductViewModel>> Get(int? id, string title, int? categoryId, int pageIndex, int pageSize)
        {
            if (categoryId == 0)
            {
                categoryId = null;
            }
            var output = await _productService.Get(id: id, isPublish: true, title: title, categoryId: categoryId, pageIndex: pageIndex, pageSize: pageSize);

            var response = new PagingViewModel<ProductViewModel>
            {
                Pages = output.Products.Select(a => ProductViewModel.FromEntity(a)).ToList(),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }

        [HttpGet("{id:int}")]
        public async Task<ProductDetailsViewModel> GetById(int id)
        {

            var output = await _productService.GetByID(id: id);

            return output;
        }
        #endregion
    }
}