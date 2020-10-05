using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class Tag : BaseEntity, IAuditableEntity
    {
        public string Title { get; set; }
    }
}
