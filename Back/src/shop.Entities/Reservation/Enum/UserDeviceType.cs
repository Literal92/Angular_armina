using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shop.Entities.Reservation.Enum
{
    public class UserDeviceType
    {
        public enum UserDeviceTypeApp : byte
        {
            customer,
            provider
        }

        public enum UserDeviceOsApp : byte
        {
            android,
            ios
        }
    }
}
