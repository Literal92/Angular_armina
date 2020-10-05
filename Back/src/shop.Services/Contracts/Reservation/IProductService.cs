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
    public interface IProductService : IDisposable
    {
        Task<(List<Product> Products, int Count, int TotalPages, HttpStatusCode status)> Get(int? id = 0,
         string title = null, string code= null, string term = null, int? categoryId = null,
         bool? isPublish = null,
         int pageIndex = 0,
          int pageSize = 0, Expression<Func<Product, object>>[] includes = null);
        Task<Product> GetById(int id = 0);
        Task<(Product product, bool success, HttpStatusCode status, string error)> Create(Product model);
        Task<ProductDetailsViewModel> GetByID(int id, bool? isPublish = null);
        Task<(Product product, string oldFile, bool success, HttpStatusCode status, string error)> Update(Product model);

        Task<(bool success, HttpStatusCode status, string error)> Delete(int id);
        Task<(int price, int unitPrice, bool success, HttpStatusCode status, string error)> ComputPrice(int id, int count);
        Task<(HttpStatusCode status, bool success, string error)> UpdateActive(int id);

    }
}
