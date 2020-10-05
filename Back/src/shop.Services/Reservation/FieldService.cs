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
    public class FieldService : IFieldService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Field> _field;


        public FieldService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _field= _uow.Set<Field>();
        }

        public async Task<(List<Field> fields, int count, int totalPages)> Get(int? id=null ,
        string title=null, int pageIndex = 0, int pageSize = 10, Expression<Func<Field, object>>[] includes = null)
        {

            var queryable = _field.AsQueryable();
            queryable = id != null ? queryable.Where(c => c.Id == id) : queryable;
            queryable = title != null && title.Trim() != "null" ? queryable.Where(c => EF.Functions.Like(c.Title, $"%{title.Trim()}%")) : queryable;
          
            queryable = queryable.OrderBy(c => c.Order);

            var count = await queryable.CountAsync();
            var totalPages = (count != 0 && pageSize != 0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize != 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            if (includes != null)
                foreach (Expression<Func<Field, object>> item in includes.ToList())
                    queryable = queryable.Include(item);

            var list = await queryable.Distinct().AsNoTracking().ToListAsync();

            return (fields: list, count: count, totalPages: totalPages);
        }
      
        public async Task<Field> GetById(int id)
        {
            return await _field.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<(Field field, bool success, HttpStatusCode status, string error)> Create(Field model)
        {
            try
            {
                var exist = await _field.FirstOrDefaultAsync(c=>c.Title.Trim() == model.Title.Trim());
                if(exist != default)
                return (null, false, HttpStatusCode.Conflict, "درج داده یکسان");

                await _field.AddAsync(model);

                await _uow.SaveChangesAsync();

                return (field: model, success: true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (field: null, success: false, HttpStatusCode.InternalServerError, error: "خطا در ثبت !");
            }
        }
        public async Task<(Field field, bool success, HttpStatusCode status, string error)> Update(Field field)
        {
            try
            {
            var find = await _field.FirstOrDefaultAsync(c => c.Id == field.Id);
            if (find == null)
                return (field: null, success: false, HttpStatusCode.NotFound, error:"ایتمی یافت نشد");
           
                _uow.Entry(find).CurrentValues.SetValues(field);
                await _uow.SaveChangesAsync();
                return (field: field, success: true, HttpStatusCode.OK, error: null); ;
            }
            catch
            {
                return (field: null, success: false , HttpStatusCode.InternalServerError, error: "خطا سمت سرور !"); 
            }

        }
        public async Task<(bool success, HttpStatusCode status, string error)> Delete(int Id)
        {
            try
            {
                var rec = await _field.FirstOrDefaultAsync(a => a.Id == Id);
                if (rec == null)
                    return (false, HttpStatusCode.NotFound, "ایتمی یافت نشد");

                rec.IsDeleted = true;
               await  _uow.SaveChangesAsync();
                return (true, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (false, HttpStatusCode.InternalServerError, "خطا در حذف");
            }
        }

        public void Dispose()
        {
            _uow.Dispose();
        }


    }
}
