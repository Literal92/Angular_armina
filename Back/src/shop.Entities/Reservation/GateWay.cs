using shop.Entities.AuditableEntity;
using shop.Entities.Identity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class GateWay : BaseEntity, IAuditableEntity
    {
        // 100 => Ok
        // <0 (منفی)=> Fail
        public int? Status { get; set; }
        // 36 char => Success
        // null => Fail
        public string Authority { get; set; }
        // میزان تراکنش
        //قیمت
        public int Amount { get; set; }
        //  شماره تراکنش
        // فکر کنم همون کد رهگیری باشه
        //Null => Fail
        public long RefId { get; set; }
        // true => Success
        // false => Fail
        public bool Success { get; set; }


        #region Relations

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }

        #endregion
    }
}
