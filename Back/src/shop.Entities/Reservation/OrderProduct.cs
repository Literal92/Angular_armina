using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class OrderProduct : BaseEntity, IAuditableEntity
    {

        public int TotalPrice { get; set; }
        // مجموع قیمت برای همکار
        public int? TotalForReseller { get; set; }
        public int UnitPrice { get; set; }
        public int Count { get; set; }

        #region Relations

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Field Field { get; set; }
        public int? fieldId { get; set; }
        public virtual ProductOption ProductOption { get; set; }
        public int? ProductOptionId { get; set; }


        public virtual OptionColor OptionColor { get; set; }
        public int? OptionColorId { get; set; }

        public virtual Product Product { get; set; }
        public int ProductId { get; set; }

        #endregion
    }
}
