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
    public interface ITagService : IDisposable
    {
        Task<(List<Tag> tags, int count, int totalPages)> Get(int? id = null,
       string title = null, int pageIndex = 0, int pageSize = 10, Expression<Func<Tag, object>>[] includes = null);

        Task<Tag> GetById(int id);
        Task<(bool success, HttpStatusCode status, string error)> CreateRange(List<Tag> model);
        Task<(Tag tag, bool success, HttpStatusCode status, string error)> Create(Tag model);
        Task<(Tag tag, bool success, HttpStatusCode status, string error)> Update(Tag tag);
        Task<(bool success, HttpStatusCode status, string error)> Delete(int Id);

    }
}
