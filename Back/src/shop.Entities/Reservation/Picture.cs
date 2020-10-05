using shop.Entities.AuditableEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class Picture : BaseEntity, IAuditableEntity
    {
        // نام
        public string Title { get; set; }
        // توضیحات
        public string Text { get; set; }
        // عکس اصلی
        public string Photo { get; set; }
        // عکس بند انگشتی
        public string Thumbnail { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        #endregion

    }
}
