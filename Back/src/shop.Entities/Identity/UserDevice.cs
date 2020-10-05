using shop.Entities.AuditableEntity;
using System;
using System.Collections.Generic;
using System.Text;
using static shop.Entities.Reservation.Enum.UserDeviceType;

namespace shop.Entities.Identity
{
    public class UserDevice : BaseEntity, IAuditableEntity
    {
        public UserDevice() { }

        public int UserId { get; set; }
        public string FcmToken { get; set; }
        public UserDeviceTypeApp Type { get; set; }
        public UserDeviceOsApp OsType { get; set; } = UserDeviceOsApp.android;
        public virtual User User { get; set; }
    }

}
