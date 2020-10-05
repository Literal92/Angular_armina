using GeoCoordinatePortable;
using Microsoft.EntityFrameworkCore;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using shop.ViewModels.Reservation;
using System.Net;
using DNTPersianUtils.Core;

namespace shop.Services.Reservation
{
    public class OrderProductService : IOrderProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<OrderProduct> _orderProduct;

        public OrderProductService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _orderProduct = _uow.Set<OrderProduct>();
        }
        /// <summary>
        /// چرا ادمین و کلاینت جدا شد؟
        /// چون نیازمند تغییرات در سمت مشتری داشتیم
        /// </summary>
        /// <returns></returns>

        public async Task<(List<OrderProduct> OrderProducts, int Count, int TotalPages, HttpStatusCode status)> Get(
            int? orderId = 0, int pageIndex = 0, int pageSize = 0,
            Expression<Func<OrderProduct, object>>[] includes = null)
        {
            var queryable = _orderProduct.AsQueryable();

            queryable = orderId != null && orderId > 0 ? queryable.Where(c => c.OrderId == orderId) : queryable;



            queryable = queryable.OrderByDescending(c => c.Id);

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize > 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (Expression<Func<OrderProduct, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.AsNoTracking().ToListAsync();

            return (OrderProducts: list, Count: count, TotalPages: totalPages, status: HttpStatusCode.OK);
        }
        public virtual async Task<OrderProduct> GetById(int id = 0)
        {
            return await _orderProduct.FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<(OrderProduct OrderProduct, bool success, HttpStatusCode status, string error)> Create(OrderProduct model)
        {
            try
            {

                await _orderProduct.AddAsync(model);
                await _uow.SaveChangesAsync();
                return (model, true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, status: HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }

        public async Task<(OrderProduct OrderProduct, bool success, HttpStatusCode status, string error)> Update(OrderProduct model)
        {
            try
            {
                var queryable = _orderProduct.AsQueryable();

                var find = await queryable.FirstOrDefaultAsync(a => a.Id == model.Id);
                if (find == null)
                    return (OrderProduct: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");


                _uow.Entry(find).CurrentValues.SetValues(model);
                await _uow.SaveChangesAsync();
                return (find, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }
        public async Task<(int price,int resellerPrice, int orderId, bool success, HttpStatusCode status, string error)> Delete(int id)
        {
            try
            {
                var queryable = _orderProduct.AsQueryable();

                var find = await queryable.Include(p=>p.Order).Include(s=>s.OptionColor).FirstOrDefaultAsync(a => a.Id == id);

                if (find == null)
                    return (price: 0, resellerPrice:0, orderId: 0, success: false, status: HttpStatusCode.NotFound, error: "ایتمی یافت نشد !");
                if (find.Order.IsReserved)
                {
                    find.OptionColor.Count += find.Count;
                }
                
                var orderId = find.OrderId;
                var totalPrice = find.TotalForReseller != null && find.TotalForReseller>0 ?
                                 find.TotalForReseller.Value:
                                 find.TotalPrice;

               // _orderProduct.Remove(find);
                 find.IsDeleted=true;
                await _uow.SaveChangesAsync();

                return (find.TotalPrice, find.TotalForReseller.Value, orderId, true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (0,0, 0, false, HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }
        public void Dispose()
        {
            _uow.Dispose();
        }
    }

}
