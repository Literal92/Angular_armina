using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class Field : BaseEntity, IAuditableEntity
    {
        public string Title { get; set; }   
        public string DisplayTitle { get; set; }   
        public FieldType FieldType { get; set; }
        public int Order { get; set; }

        #region Relations
        public virtual ICollection<ProductOption> ProductOptions { get; set; }
        #endregion
    }
}
