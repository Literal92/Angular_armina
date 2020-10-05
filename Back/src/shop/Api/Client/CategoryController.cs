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

namespace shop.Api.Client
{
    [Route("api/client/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "v1.0")]

    public class CategoryController : Controller
    {
        #region Fields
        private readonly ICategoryService _categoryService;
        #endregion

        #region Ctor
        public CategoryController(ICategoryService categoryService, 
        IUploadService uploadService,
         IWebHostEnvironment hostingEnvironment)
        {
            _categoryService = categoryService;
        }
        #endregion

        #region Methods    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isChkParent"></param>
        /// <param name="contionRoot"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagingViewModel<CategoryViewModel>> Get(string title, int? id, int pageIndex, int pageSize)
        {
            var output = await _categoryService.Get(id: id, title: title, contionRoot:false,  pageIndex: pageIndex, pageSize: pageSize);

            var response = new PagingViewModel<CategoryViewModel>
            {
                Pages = output.categories.Select(a => CategoryViewModel.FromEntity(a)).ToList(),
                TotalPage = output.totalPages,
                Count = output.count
            };

            return response;
        }

        #endregion
    }
}