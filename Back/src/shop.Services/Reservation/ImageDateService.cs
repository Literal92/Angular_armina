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
    public class ImageDateService : IImageDateService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ImageDate> _imageDates;

        public ImageDateService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _imageDates = _uow.Set<ImageDate>();

        }
       public async Task<(List<ImageDate> ImageDates,int Count, int TotalPages)> Get(
           int? id = 0, DateTime? date = null, int pageIndex=0, int pageSize=0)
        {
            var queryable = _imageDates.AsQueryable();
            queryable = id != null && id > 0 ? queryable.Where(c => c.Id == id) : queryable;
            queryable = date != null ? queryable.Where(c => c.Date.Date == date.Value.Date) : queryable;

            var count = await queryable.CountAsync();
            var totalPages=(count!=0 && pageSize!=0) ? Convert.ToInt32(Math.Ceiling((double)count / pageSize)) : count;

            if (pageIndex > 0 && pageSize > 0)
            {
                int skip = (pageIndex - 1) * pageSize;
                skip = skip <= 0 ? 0 : skip;
                int take = pageSize;
                queryable = queryable.Skip(skip).Take(take);
            }
            var list = await queryable.AsNoTracking().ToListAsync();

            return (ImageDates: list, Count: count, TotalPages: totalPages);

        }

        public async Task<(ImageDate ImageDates, bool success, HttpStatusCode status,string error)> Create(ImageDate model)
        {
            try
            {
                var find = await _imageDates.FirstOrDefaultAsync(c => c.Date.Date == model.Date.Date);
                if (find != null)
                {
                    find.Images.AddRange(model.Images);
                    _uow.Entry(find).State = EntityState.Modified;
                    model.Id = find.Id;
                }
                else
                    await _imageDates.AddAsync(model);

                await _uow.SaveChangesAsync();

                return (model, true, HttpStatusCode.OK, null);
            }
            catch(Exception ex)
            {
                return (null, false, HttpStatusCode.InternalServerError, "خطا سمت سرور!");
            }
        }

       public async Task<(int id, List<string> oldFiles, bool success, HttpStatusCode status, string error)> Delete(int? id=null, 
                                                                          DateTime? date= null, string fileName=null)
        {
            try
            {
                if (id!= null)
                {
                    var findById = await _imageDates.FirstOrDefaultAsync(c => c.Id == id.Value);

                    if (findById == null)
                        return (0, null, false, HttpStatusCode.NotFound, "موردی یافت نشد !");

                    var oldFilesById = findById.Images;

                    _imageDates.Remove(findById);

                    await _uow.SaveChangesAsync();

                    return (findById.Id, oldFilesById, true, HttpStatusCode.OK, null);
                }

                var find = await _imageDates.FirstOrDefaultAsync(c => c.Date.Date == date.Value.Date);
                if (find == null)
                    return (0, null, false, HttpStatusCode.NotFound, "موردی یافت نشد !");

                if (!find.Images.Any(c => c.Trim() == fileName.Trim()))
                    return (0, null, false, HttpStatusCode.NotFound, "موردی یافت نشد !");


                var oldFile = new List<string> { fileName };

                if (find.Images.Count > 1)
                {
                    find.Images.Remove(fileName);
                    _uow.Entry(find).State = EntityState.Modified;
                }
                // اگر یک عکس داشت کل رکورد حذف شود
                else
                    _imageDates.Remove(find);

                await _uow.SaveChangesAsync();

                return (find.Id, oldFile, true, HttpStatusCode.OK, null);
            }
            catch(Exception ex)
            {
                return (0, null, false, HttpStatusCode.InternalServerError, "خطا سمت سرور!");
            }


        }
        public void Dispose()
        {
            _uow.Dispose();
        }
    }

}
