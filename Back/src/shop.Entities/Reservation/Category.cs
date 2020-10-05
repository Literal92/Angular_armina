using shop.Entities.AuditableEntity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class Category : BaseEntity, IAuditableEntity
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Thumbnail { get; set; }
        public string Path { get; set; }
        public int Order { get; set; }
        #region Relations
        public int? ParentId { get; set; }
        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> SubCategory { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        #endregion
    }
}
