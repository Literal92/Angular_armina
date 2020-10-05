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
    public interface IProductOptionService : IDisposable
    {
        Task<(List<ProductOption> options, int Count, int TotalPages)> Get(int? id = null,
        int? fieldId = null ,string title = null, int? productId=null,
        int pageIndex = 0, int pageSize = 10,
        Expression<Func<ProductOption, object>>[] includes = null);
        Task<List<ProductOption>> GetByIds(List<int> Ids = null, Expression<Func<ProductOption, object>>[] includes = null);
        Task<ProductOption> GetById(int id);
        Task<(ProductOption option, HttpStatusCode status, bool success, string error)> Create(ProductOption model);
        Task<(ProductOption option, bool success, HttpStatusCode status, string error)> Update(ProductOption option);
        Task<(bool success, HttpStatusCode status, string error)> Remove(int id);
        Task<(int price, int priceReseller, int unitPrice, bool success, HttpStatusCode status, string error)> ComputPrice(int productId, int optionId, int? optionColorId = null, int count = 0, bool isReseller = false);

    }
}
