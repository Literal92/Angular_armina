using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shop.Entities.AuditableEntity;
using shop.Entities.Identity;
using shop.Entities.Reservation.Enum;

namespace shop.Entities.Reservation
{
    public class Product : BaseEntity, IAuditableEntity
    {

        public string Title { get; set; }
        public string Pic { get; set; }
        public string Thumbnail { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }

        public bool IsPublish { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        // کد محصول
        public string Code { get; set; }
        #region Relations
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int? ParentId { get; set; }
        public virtual Product Parent { get; set; }
        public virtual ICollection<Product> ChildProduct { get; set; }
        public virtual ICollection<ProductOption> ProductOptions { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        #endregion
    }
}