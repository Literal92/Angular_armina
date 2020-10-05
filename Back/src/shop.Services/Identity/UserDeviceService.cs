using Microsoft.EntityFrameworkCore;
using shop.Common.GuardToolkit;
using shop.DataLayer.Context;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.Services.Contracts.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static shop.Entities.Reservation.Enum.UserDeviceType;

namespace shop.Services.Reservation
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IUnitOfWork _uow;
        readonly DbSet<UserDevice> _userDevice;

        public UserDeviceService(IUnitOfWork uow)
        {
            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));
            _userDevice = _uow.Set<UserDevice>();

        }
        public async Task<UserDevice> GetById(int id, Expression<Func<UserDevice, object>>[] includes = null)
        {
            var queryable = _userDevice.AsQueryable();
            if (includes != null)
                foreach (var item in includes)
                    queryable = queryable.Include(item);
            return await queryable.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<UserDevice> Creat(UserDevice data)
        {
            await _userDevice.AddAsync(data);
            await _uow.SaveChangesAsync();
            return data;
        }


        public async Task<UserDevice> Update(UserDevice data)
        {
            var find = await _userDevice.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (find == null)
                return null;

            _uow.Entry(find).CurrentValues.SetValues(data);
            await _uow.SaveChangesAsync();
            return data;
        }

        public void Dispose()
        {
            _uow.Dispose();
        }

        public async Task<UserDevice> GetByToken(string fmcToken, UserDeviceTypeApp Type)
        {
            var queryable = _userDevice.AsQueryable();
            return await queryable.FirstOrDefaultAsync(c => c.FcmToken.Equals(fmcToken.Trim()) && c.Type == Type);
        }

        public async Task<IList<UserDevice>> Get(int userId, UserDeviceTypeApp Type)
        {
            var queryable = _userDevice.AsQueryable();
            var f =  queryable.Where(c => c.UserId == userId && c.Type == Type);
            var d= f.ToList();
            return d;
        }

        public async Task<bool> Delete(int id)
        {
            var find = await _userDevice.FirstOrDefaultAsync(c => c.Id == id);
            if (find == null)
                return false;

            _userDevice.Remove(find);
            await _uow.SaveChangesAsync();
            return true;
        }

    }
}
