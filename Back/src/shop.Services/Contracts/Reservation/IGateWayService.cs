using shop.Entities.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace shop.Services.Contracts.Reservation
{
    public interface IGateWayService : IDisposable
    {
        Task<GateWay> GetById(int id, Expression<Func<GateWay, object>>[] includes = null);

        Task<GateWay> Create(GateWay gateWay);
        Task<GateWay> Update(int Id, GateWay gateWay);

    }
}
