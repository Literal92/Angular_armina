using shop.Entities.Reservation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Contracts.Reservation
{
    public interface ITrackingCodeService : IDisposable
    {

        Task<(TrackingCode trackingCode, bool success, HttpStatusCode status, string error)> Create(TrackingCode trackingCode);

        Task<(List<TrackingCode> trackingCode, bool success, HttpStatusCode status, string error)> Create(List<TrackingCode> trackingCode);

        Task<(List<TrackingCode> TrackingCodes, int Count, int TotalPages)> Get(string
              name, string city = null,
              int pageIndex = 0,
              int pageSize = 0, Expression<Func<TrackingCode, object>>[] includes = null);

    }
}
