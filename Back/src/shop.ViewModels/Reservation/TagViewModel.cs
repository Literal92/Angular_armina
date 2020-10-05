
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class TagViewModel : BaseDto<TagViewModel, Tag>
    {

        public string Title { get; set; }

    }




}