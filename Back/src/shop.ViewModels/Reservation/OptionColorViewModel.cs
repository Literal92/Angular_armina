
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class OptionColorViewModel : BaseDto<OptionColorViewModel, OptionColor>
    {

        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        // قیمت همکار
        public int? ResellerPrice { get; set; }
        //کدرنگ
        public string Code { get; set; }
        public int Order { get; set; }

        #region Relations
        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; }

        #endregion

    }


    public class OptionColorSimpleViewModel : BaseDto<OptionColorSimpleViewModel, OptionColor>
    {
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        // قیمت همکار
        public int? ResellerPrice { get; set; }
        //کدرنگ
        public string Code { get; set; }
        public int Order { get; set; }
    }


}