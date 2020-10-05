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
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Reservation
{
    public class OptionColorService : IOptionColorService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<OptionColor> _optionColors;

        public OptionColorService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _optionColors = _uow.Set<OptionColor>();
        }

        public async Task<(List<OptionColor> colors, int Count, int TotalPages)> Get(int? id = null,
         int? producOptionId = null, string title = null,
         int? fieldId = null, int pageIndex = 0, int pageSize = 10,
         Expression<Func<OptionColor, object>>[] includes = null)
        {

            var queryable = _optionColors.AsQueryable();
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;
            queryable = title != null && title.Trim() != "null" ? queryable.Where(c => EF.Functions.Like(c.Title, $"%{title.Trim()}%")) : queryable;
            queryable = producOptionId != null ? queryable.Where(c => c.ProductOptionId == producOptionId.Value) : queryable;

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            queryable = queryable.OrderBy(c => c.Order);

            if (pageIndex > 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            var list = await queryable.Distinct().AsNoTracking().ToListAsync();

            return (colors: list, Count: count, TotalPages: totalPages);
        }

        public async Task<OptionColor> GetById(int id)
        {
            return await _optionColors.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<(OptionColor option, HttpStatusCode status, bool success, string error)> Create(OptionColor model)
        {
            try
            {
                var exist = await _optionColors.FirstOrDefaultAsync(c => c.Title.Trim() == model.Title.Trim() && c.ProductOptionId == model.ProductOptionId);
                if (exist != default)
                    return (null, HttpStatusCode.Conflict, false, "درج داده یکسان");

                await _optionColors.AddAsync(model);
                await _uow.SaveChangesAsync();
                return (option: model, status: HttpStatusCode.OK, success: true, error: null);
            }
            catch (Exception ex)
            {
                return (option: null, status: HttpStatusCode.InternalServerError, success: false, error: "خطا در ثبت !");
            }
        }

        public async Task<(OptionColor option, bool success, HttpStatusCode status, string error)> Update(OptionColor option)
        {
            try
            {
                var queryable = _optionColors.AsQueryable();

                var find = await queryable.FirstOrDefaultAsync(a => a.Id == option.Id);
                if (find == null)
                    return (option: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                _uow.Entry(find).CurrentValues.SetValues(option);
                await _uow.SaveChangesAsync();
                return (find, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }


        public async Task<(OptionColor option, bool success, HttpStatusCode status, string error)> Update(int id, int count)
        {
            try
            {
                var queryable = _optionColors.AsQueryable();

                var find = await queryable.FirstOrDefaultAsync(a => a.Id == id);
                if (find == null)
                    return (option: null, success: false, status: HttpStatusCode.NotFound, "موردی یافت نشد !");

                find.Count += count;
                await _uow.SaveChangesAsync();
                return (find, success: true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (null, success: false, HttpStatusCode.InternalServerError, error: "خطا سمت سرور !");
            }
        }



        public async Task<(bool success, HttpStatusCode status, string error)> Remove(int id)
        {
            try
            {

                var item = await _optionColors.FirstOrDefaultAsync(a => a.Id == id);
                if (item == null)
                    return (success: false, status: HttpStatusCode.NotFound, error: "ایتمی یافت نشد !");

                item.IsDeleted = true;

                await _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا سمت سرور !");
            }
        }



        public void Dispose()
        {
            _uow.Dispose();
        }


    }
}