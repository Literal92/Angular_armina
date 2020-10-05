using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shop.Entities.Identity.Enum
{
    public class UserTypeEnum
    {
        public enum UserType : byte
        {
           // Seller,
            Reseller=0,
            Client=1
        }
    }
}
