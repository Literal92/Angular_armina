using shop.Entities.Identity;
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
    public interface IOrderService : IDisposable
    {
        Task<(List<Order> Orders, int Count, int TotalPages, HttpStatusCode status)> Get(
            int? id = 0,
          int? userId = null,
         string userName = null,
         string reciverName = null,
            string reciverMobile = null,
            string senderName = null,
            string senderMobile = null,
            string address = null,
         string productName = null,
         OrderSendType? orderSendType = null,

         OrderType? orderType = null,
         DateTime? from = null, DateTime? to = null,
         int pageIndex = 0, int pageSize = 0, Expression<Func<Order, object>>[] includes = null);
        Task<Order> GetById(int id = 0, Expression<Func<Order, object>>[] includes = null);
        Task<(Order product, bool success, HttpStatusCode status, string error)> Create(Order model);

        Task<(Order product, string oldFile, bool success, HttpStatusCode status, string error)> Update(Order model);
        Task<(Order order, bool success, HttpStatusCode status, string error)> SaveBasket(Order model, int userId,List<Role> roles);

        Task<(Order product, string oldFile, bool success, HttpStatusCode status, string error)> ChangeSender(int id);
        Task<(bool success, HttpStatusCode status, string error)> Delete(int id);
        Task<OrderBasketViewModel> GetByUserID(int userId, OrderType? orderType = null, bool isReseller = false);
        Task<(int totalPrice, int discountPrice, int totalWithDiscountPrice)> ChangePrice(int id, int decresePrice, int userId);
        Task CancellReserve();
        Task<List<GetOrderDontSendWithAddressViewModel>> GetOrderDontSendWithAddress();
        Task<(bool success, HttpStatusCode status, string error)> EditAddress(EditAddressViewModel model);
        Task<(bool success, HttpStatusCode status, string error)> DeleteByOrderProductId(int id);
        Task<(bool success, HttpStatusCode status, string error)> AcceptPaymnet(int id, bool accept);
        Task<List<ReportViewModel>> GetReportDaily(DateTime? from, DateTime? to);
        Task<List<ReportMonthViewModel>> GetReportMonthly(int? from, int? to, int year);
        Task<bool> SetShamsiDate();
    }

}
