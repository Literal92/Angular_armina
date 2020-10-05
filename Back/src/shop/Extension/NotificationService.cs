using shop.Common.Notification;
using shop.Services.Contracts.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static shop.Entities.Reservation.Enum.UserDeviceType;

namespace shop.Extension
{
    public class NotificationService
    {
        public readonly IUserDeviceService _userDevice;
        public NotificationService(IUserDeviceService userDevice)
        {
            _userDevice = userDevice;
        }
        public async void SendNotification(int UserId, UserDeviceTypeApp Type, string title, string msg, string id, string kind)
        {
            var list = await _userDevice.Get(UserId, Type);
            if (list.Any())
            {
                var result = Notification.Send(list.Select(a => a.FcmToken).ToList(), title, msg, id, kind);
            }
            //foreach (var item in list)
            //{
            //    var result = Notification.Send(item.FcmToken, title, msg, id, kind);
            //    if (result != "-1")
            //    {
            //        await _userDevice.Delete(item.Id);
            //    }
            //}
        }
    }
}
