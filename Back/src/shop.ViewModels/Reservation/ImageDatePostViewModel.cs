
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class ImageDatePostViewModel
    {
        public string Date { get; set; }
        public List<IFormFile> Images { get; set; }

    }
   

   

}