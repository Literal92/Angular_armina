
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System;
using System.Collections.Generic;
using DNTPersianUtils.Core;

namespace shop.ViewModels.Reservation
{
    public class ImageDateViewModel :BaseDto<ImageDateViewModel,ImageDate>
    {
        public DateTime Date { get; set; }

        public string DatePersian { 
            get {
                var datePer = Date.ToShortPersianDateString();
                return datePer;
            }
        }
        public List<string> Images { get; set; }
        public int Count { 
            get {
                var count = Images != null ? Images.Count : 0;
                return count;
            }
        }



    }
   

   

}