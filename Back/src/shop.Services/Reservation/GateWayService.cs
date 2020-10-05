using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using shop.Services.Contracts.Reservation;
using shop.DataLayer.Context;
using shop.Entities.Reservation;
using shop.Common.GuardToolkit;

namespace shop.Services.Reservation
{
    public class GateWayService : IGateWayService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<GateWay> _gateWays;



        public GateWayService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _gateWays = _uow.Set<GateWay>();
        }


        public async Task<GateWay> GetById(int id, Expression<Func<GateWay, object>>[] includes = null) {
            var queryable = _gateWays.AsQueryable();
            queryable = queryable.Where(c => c.Id == id);
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<GateWay> Create(GateWay gateWay)
        {
            try
            {
                await _gateWays.AddAsync(gateWay);
                await _uow.SaveChangesAsync();
                return gateWay;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<GateWay> Update(int Id, GateWay gateWay)
        {
            try
            {
                var item = await _gateWays.FirstOrDefaultAsync(a => a.Id == Id);
                if (item == null)
                    return null;
                // update extraTime
                _uow.Entry(item).CurrentValues.SetValues(gateWay);
                await _uow.SaveChangesAsync();
                return gateWay;
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }
        public void Dispose()
        {
            _uow.Dispose();
        }
    }
}
