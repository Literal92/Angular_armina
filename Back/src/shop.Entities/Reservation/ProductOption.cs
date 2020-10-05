using shop.Entities.AuditableEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class ProductOption : BaseEntity, IAuditableEntity
    {
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; } 
        public int Order { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int  FieldId { get; set; }
        public virtual Field  Field { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<OptionColor> OptionColors { get; set; }



        #endregion

    }
}
