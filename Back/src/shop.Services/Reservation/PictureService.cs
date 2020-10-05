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
    public class PictureService : IPictureService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Picture> _picture;


        public PictureService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _picture = _uow.Set<Picture>();
        }

      
        public async Task<Picture> GetById(int id)
        {
            return await _picture.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<(Picture picture, bool success, HttpStatusCode status, string error)> Create(Picture model)
        {
            try
            {

                await _picture.AddAsync(model);

                await _uow.SaveChangesAsync();

                return (picture: model, success: true, HttpStatusCode.OK, error: null);
            }
            catch (Exception ex)
            {
                return (picture: null, success: false, HttpStatusCode.InternalServerError, error: "خطا در ثبت !");
            }
        }
        public async Task<(bool success, int pId, string oldFile, string oldThumbnail, HttpStatusCode status, string error)> Delete(int Id)
        {
            try
            {
                var rec = await _picture.FirstOrDefaultAsync(a => a.Id == Id);
                if (rec == null)
                    return (false,0, null, null, HttpStatusCode.NotFound, "ایتمی یافت نشد");

                rec.IsDeleted = true;
                var pId = rec.ProductId;
                var file = rec.Photo;
                var thumbnail = rec.Thumbnail;
               await  _uow.SaveChangesAsync();
                return (true, pId, rec.Photo, rec.Thumbnail, HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return (false,0,null, null, HttpStatusCode.InternalServerError, "خطا در حذف");
            }
        }

        public void Dispose()
        {
            _uow.Dispose();
        }


    }
}
