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
    public interface IFieldService : IDisposable
    {
        Task<(List<Field> fields, int count, int totalPages)> Get(int? id=null ,
        string title=null, int pageIndex = 0, int pageSize = 10, Expression<Func<Field, object>>[] includes = null);
      
        Task<Field> GetById(int id);
        Task<(Field field, bool success, HttpStatusCode status, string error)> Create(Field model);
        Task<(Field field, bool success, HttpStatusCode status, string error)> Update(Field field);
        Task<(bool success, HttpStatusCode status, string error)> Delete(int Id);
    }
}
