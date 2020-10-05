using Microsoft.AspNetCore.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Contracts.Reservation
{
    public interface IPictureService : IDisposable
    {
        Task<Picture> GetById(int id);
        Task<(Picture picture, bool success, HttpStatusCode status, string error)> Create(Picture model);
        Task<(bool success, int pId, string oldFile, string oldThumbnail, HttpStatusCode status, string error)> Delete(int Id);
    }
}
