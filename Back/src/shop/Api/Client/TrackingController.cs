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

    public class TrackingController : Controller
    {
        #region fields
        private readonly ITrackingCodeService _trackingCodeService;
        private readonly int _userId;

        #endregion

        #region Ctor
        public TrackingController(ITrackingCodeService trackingCodeService, IHttpContextAccessor httpContextAccessor)
        {
            _trackingCodeService = trackingCodeService;
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
        public async Task<PagingViewModel<TrackingCodeViewModel>> Get(string title, string city, int pageIndex, int pageSize)
        {
           
            var output = await _trackingCodeService.Get(name:title,city:city, pageIndex: 0, pageSize: 100);

            var response = new PagingViewModel<TrackingCodeViewModel>
            {
                Pages = output.TrackingCodes.Select(a => TrackingCodeViewModel.FromEntity(a)).ToList(),
                TotalPage = output.TotalPages,
                Count = output.Count
            };

            return response;
        }



        #endregion
    }
}