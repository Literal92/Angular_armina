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
    public interface IOrderProductService : IDisposable
    {
        Task<(List<OrderProduct> OrderProducts, int Count, int TotalPages, HttpStatusCode status)> Get(
              int? orderId = 0, int pageIndex = 0, int pageSize = 0,
              Expression<Func<OrderProduct, object>>[] includes = null);
        Task<OrderProduct> GetById(int id = 0);
        Task<(OrderProduct OrderProduct, bool success, HttpStatusCode status, string error)> Create(OrderProduct model);

        Task<(OrderProduct OrderProduct, bool success, HttpStatusCode status, string error)> Update(OrderProduct model);
         Task<(int price, int resellerPrice, int orderId, bool success, HttpStatusCode status, string error)> Delete(int id);
    }
}
