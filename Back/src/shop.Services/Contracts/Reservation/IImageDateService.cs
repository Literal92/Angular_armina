using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels.Reservation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Contracts.Reservation
{
    public interface IImageDateService : IDisposable
    {
        Task<(List<ImageDate> ImageDates, int Count, int TotalPages)> Get(
            int? id = 0, DateTime? date = null, int pageIndex = 0, int pageSize = 0);
        Task<(ImageDate ImageDates, bool success, HttpStatusCode status,string error)> Create(ImageDate model);
        Task<(int id, List<string> oldFiles, bool success, HttpStatusCode status, string error)> Delete(int? id = null,
                                                                          DateTime? date = null, string fileName = null);


    }
}
