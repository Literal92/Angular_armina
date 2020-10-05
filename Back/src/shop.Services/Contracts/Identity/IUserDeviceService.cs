using shop.Entities.Identity;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static shop.Entities.Reservation.Enum.UserDeviceType;

namespace shop.Services.Contracts.Reservation
{
    public interface IUserDeviceService : IDisposable
    {
        Task<UserDevice> GetById(int id, Expression<Func<UserDevice, object>>[] includes = null);
        Task<UserDevice> GetByToken(string fmcToken,UserDeviceTypeApp Type);
        Task<IList<UserDevice>> Get(int userId,UserDeviceTypeApp Type);
        Task<UserDevice> Creat(UserDevice data);
        Task<UserDevice> Update(UserDevice data);
        Task<bool> Delete(int id);

    }
}
