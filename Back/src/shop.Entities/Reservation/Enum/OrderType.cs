using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shop.Entities.Reservation.Enum
{
    public enum OrderType
    {
        [Display(Name = "در انتظار پرداخت")]
        Unpaid=0,
        [Display(Name = "پرداخت شده")]
        Paid=1,
        [Display(Name = "لغو شده")]
        Canceled=2
    }

    public enum OrderSendType
    {
        [Display(Name = "معلق")]
        Pending = 0,
        [Display(Name = "بسته بندی")]
        Accepted = 1,
        [Display(Name = "ارسال شده")]
        Posted = 2
    }
}
