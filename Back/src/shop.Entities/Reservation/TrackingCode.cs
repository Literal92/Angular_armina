using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class TrackingCode : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public DateTime? SendDate { get; set; }

    }
}
