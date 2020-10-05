using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class ProductTag : BaseEntity, IAuditableEntity
    {
        #region Relation
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        #endregion
    }
}
