using shop.Entities.AuditableEntity;
using shop.Entities.Identity;
using shop.Entities.Reservation.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace shop.Entities.Reservation
{
    public class Order : BaseEntity, IAuditableEntity
    {
        public int TotalPrice { get; set; }
        public int TotalWithDiscountPrice { get; set; }
        // مبلغ تخفیف گرفته
        public int DiscountPrice { get; set; }
        //public int? ShippingCost { get; set; }

        // درصد تخفیف گرفته
        public int DiscountPercent { get; set; }
        /// <summary>
        /// تاریخ پرداخت هزینه سفارش
        /// </summary>
        public DateTime? PaidDateTime { get; set; }
        public string Picture { get; set; }

        // تایید پرداخت
        public bool AcceptPayment { get; set; }

        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string ReciverMobile { get; set; }
        public string ReciverName { get; set; }
        public string ReciverAddress { get; set; }
        public long? RefId { get; set; }
        /// <summary>
        /// وضعیت پرداخت
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// وضعیت ارسال شدن
        /// </summary>
        public OrderSendType OrderSendType { get; set; }

        // ایا رزرو شده است؟
        public bool IsReserved { get; set; }
        // تا چه زمانی
        // در کد زمان اکنون + یک ساعت دیگه میشه
        public DateTime? ReserveTo { get; set; }
        /// <summary>
        ///  ماه در سال به شمسی
        /// </summary>
        public int? MonthOfYear { get; set; }
        ///<summary>
        /// سال به شمسی
        ///</summary>
        public int? Year { get; set; }

        #region Relations
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<GateWay> GateWays { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        #endregion
    }
}
