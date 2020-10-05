using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Reservation
{
    public class TrackingCodeService : ITrackingCodeService
    {
        private readonly IUnitOfWork _uow;
        readonly DbSet<TrackingCode> _trackingCode;
        public TrackingCodeService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _trackingCode = _uow.Set<TrackingCode>();
        }



        public void Dispose()
        {
            _uow.Dispose();
        }

        public async Task<(TrackingCode trackingCode, bool success, HttpStatusCode status, string error)> Create(TrackingCode trackingCode)
        {
            try
            {

                await _trackingCode.AddAsync(trackingCode);
                await _uow.SaveChangesAsync();
                return (trackingCode: trackingCode, success: true, HttpStatusCode.OK, null);
            }
            catch
            {
                return (trackingCode: null, success: false, status: HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");

            }
        }

        public async Task<(List<TrackingCode> trackingCode, bool success, HttpStatusCode status, string error)>
            Create(List<TrackingCode> trackingCode)
        {
            try
            {
                await _trackingCode.AddRangeAsync(trackingCode);
                await _uow.SaveChangesAsync();
                return (trackingCode: trackingCode, success: true, HttpStatusCode.OK, null);
            }
            catch
            {
                return (trackingCode: null, success: false, status: HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");

            }
        }




        public async Task<(List<TrackingCode> TrackingCodes, int Count, int TotalPages)> Get(string
               name, string city = null,
               int pageIndex = 0,
               int pageSize = 0, Expression<Func<TrackingCode, object>>[] includes = null)
        {

            var queryable = _trackingCode.AsQueryable();


            queryable = name != null && name != "null" ?
                queryable.Where(c => EF.Functions.Like(c.Name, $"%{name.Trim()}%")) : queryable;


            queryable = queryable.OrderByDescending(c => c.Id);

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : 0;

            if (pageIndex >= 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            var list = await queryable.AsNoTracking().ToListAsync();
            return (TrackingCodes: list, Count: count, TotalPages: totalPages);

        }
    }
}
