using shop.Entities.AuditableEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class OptionColor : BaseEntity, IAuditableEntity
    {
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; } 
        //کدرنگ
        public string Code { get; set; }
        public int Order { get; set; }
        // قیمت همکار
        public int ResellerPrice { get; set; }

        #region Relations
        public int ProductOptionId { get; set; }
        public virtual ProductOption ProductOption { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }



        #endregion

    }
}
