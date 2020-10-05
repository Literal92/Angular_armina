
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class OrderProductViewModel : BaseDto<OrderProductViewModel, OrderProduct>
    {

        public int TotalPrice { get; set; }
        // مجموع قیمت برای همکار
        public int? TotalForReseller { get; set; }
        public int UnitPrice { get; set; }
        public int Count { get; set; }

        #region Relations

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public virtual FieldViewModel Field { get; set; }
        public int? fieldId { get; set; }
        public virtual ProductOption ProductOption { get; set; }
        public int? ProductOptionId { get; set; }


        public virtual OptionColor OptionColor { get; set; }
        public int? OptionColorId { get; set; }

        public virtual ProductViewModel Product { get; set; }
        public int ProductId { get; set; }

        #endregion
    }


    public class OrderProductForBasketViewModel : BaseDto<OrderProductForBasketViewModel, OrderProduct>
    {

        public int TotalPrice { get; set; }
        // مجموع قیمت برای همکار
        public int? TotalForReseller { get; set; }
        public int UnitPrice { get; set; }
        public int Count { get; set; }
      
        #region Relations

        public int OrderId { get; set; }

        public virtual FieldForBasketViewModel Field { get; set; }
        public int? fieldId { get; set; }
        public virtual OptionColorViewModel OptionColor { get; set; }
        public int? OptionColorId { get; set; }
        public virtual ProductForBasketViewModel Product { get; set; }
        public int ProductId { get; set; }


        public virtual ProductOptionViewModel ProductOption { get; set; }
        public int? ProductOptionId { get; set; }

        #endregion
    }
}