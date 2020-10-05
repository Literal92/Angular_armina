
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;

namespace shop.ViewModels.Reservation
{
    public class OrderViewModel : BaseDto<OrderViewModel, Order>
    {
        public int TotalPrice { get; set; }
        public int TotalWithDiscountPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int DiscountPercent { get; set; }
        public long? RefId { get; set; }
        public bool AcceptPayment { get; set; }

        /// <summary>
        /// تاریخ پرداخت هزینه سفارش
        /// </summary>
        public DateTime? PaidDateTime { get; set; }
        [NotMapped]
        public string PaidFaDateTime
        {
            get
            {
                var toFa = PaidDateTime?.ToShortPersianDateTimeString();
                return toFa;
            }
        }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string ReciverMobile { get; set; }
        public string ReciverName { get; set; }
        public string ReciverAddress { get; set; }
        public OrderType OrderType { get; set; }
        public OrderSendType OrderSendType { get; set; }
        // ایا رزرو شده است؟
        public bool IsReserved { get; set; }
        // در کد زمان اکنون + یک ساعت دیگه میشه
        public DateTime? ReserveTo { get; set; }
        public string ReserveToFa
        {
            get
            {
                var toFa = ReserveTo?.ToShortPersianDateTimeString();
                return toFa;
            }
        }
        public string Picture { get; set; }

        #region Relations        
        public virtual ICollection<OrderProductViewModel> OrderProducts { get; set; }
        public UserViewModel User { get; set; }
        public int UserId { get; set; }
        #endregion
    }
    public class OrderBasketViewModel : BaseDto<OrderBasketViewModel, Order>
    {

        public int TotalPrice { get; set; }
        public int TotalWithDiscountPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int DiscountPercent { get; set; }
        // ایا رزرو شده است؟
        public bool IsReserved { get; set; }
        // تا چه زمانی
        // در کد زمان اکنون + یک ساعت دیگه میشه
        public DateTime? ReserveTo { get; set; }

        [NotMapped]
        public string ReserveToFa
        {
            get
            {
                var toFa = ReserveTo?.ToShortPersianDateTimeString();
                return toFa;
            }
        }

        /// <summary>
        /// 
        /// تاریخ پرداخت هزینه سفارش
        /// </summary>
        public DateTime? PaidDateTime { get; set; }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string ReciverMobile { get; set; }
        public string ReciverName { get; set; }
        public string ReciverAddress { get; set; }
        public OrderType OrderType { get; set; }
        public OrderSendType OrderSendType { get; set; }


        #region Relations        
        public virtual ICollection<OrderProductForBasketViewModel> OrderProducts { get; set; }
        #endregion
    }


    public class OrderBasketSaveViewModel : BaseDto<OrderBasketSaveViewModel, Order>
    {

        public int TotalPrice { get; set; }
        public int TotalWithDiscountPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int DiscountPercent { get; set; }

        /// <summary>
        /// 
        /// تاریخ پرداخت هزینه سفارش
        /// </summary>
        public DateTime? PaidDateTime { get; set; }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string ReciverMobile { get; set; }
        public string ReciverName { get; set; }
        public string ReciverAddress { get; set; }
        public OrderType OrderType { get; set; }
        public OrderSendType OrderSendType { get; set; }

        // ای رزرو شده است ؟
        public bool IsReserved { get; set; } = false;
        public string Picture { get; set; }

        #region Relations        
        public virtual ICollection<OrderProductViewModel> OrderProducts { get; set; }
        #endregion
    }
    public class RemoveItemBasketViewModel
    {
        public int TotalPrice { get; set; }
        public int DiscountPrice { get; set; }

        public int totalWithDiscountPrice { get; set; }
    }

    public class GetOrderDontSendWithAddressViewModel : BaseDto<GetOrderDontSendWithAddressViewModel, Order>
    {

        public string ReciverAddress { get; set; }
        public string ReciverName { get; set; }
        public string ReciverMobile { get; set; }
        #region Relations        
        public virtual ICollection<OrderProductForBasketViewModel> OrderProducts { get; set; }
        #endregion
    }
    public class EditAddressViewModel
    {
        public int Id { get; set; }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string ReciverMobile { get; set; }
        public string ReciverName { get; set; }
        public string ReciverAddress { get; set; }
    }

    public class AcceptOrderViewModel
    {
        public int Id { get; set; }
        public bool Accept { get; set; }

    }
    public class ReportViewModel {
        public int Day { get; set; }
        public DateTime DateTime { get; set; }
        public string DateFa {
            get {
                return DateTime.ToShortPersianDateString();
            }
        }
        public int Count { get; set; }
    }
    public class ReportMonthViewModel
    {
        public int Month { get; set; }
        public int Count { get; set; }
    }
}