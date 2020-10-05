using Microsoft.AspNetCore.Http;
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
    public interface IOptionColorService : IDisposable
    {

        Task<(List<OptionColor> colors, int Count, int TotalPages)> Get(int? id = null,
         int? producOptionId = null, string title = null,
         int? fieldId = null, int pageIndex = 0, int pageSize = 10,
         Expression<Func<OptionColor, object>>[] includes = null);

        Task<OptionColor> GetById(int id);
        Task<(OptionColor option, HttpStatusCode status, bool success, string error)> Create(OptionColor model);
        Task<(OptionColor option, bool success, HttpStatusCode status, string error)> Update(OptionColor option);
        Task<(bool success, HttpStatusCode status, string error)> Remove(int id);
         Task<(OptionColor option, bool success, HttpStatusCode status, string error)> Update(int id, int count);

    }
}
