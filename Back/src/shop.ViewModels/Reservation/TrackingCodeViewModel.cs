
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace shop.ViewModels.Reservation
{
    public class TrackingCodeViewModel : BaseDto<TrackingCodeViewModel, TrackingCode>
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public DateTime? SendDate { get; set; }


        public DateTime? SendDateFa { get; set; }
        [NotMapped]
        public string PaidFaDateTime
        {
            get
            {
                var toFa = SendDate?.ToPersianDateTextify();
                return toFa;
            }
        }


    }
    public class TrackingCodeFileViewModel 
    {
        public string FileName { get; set; }
        public string Description { get; set; }

        public string SendDate { get; set; }

    }


}