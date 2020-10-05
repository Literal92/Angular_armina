using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shop.Entities.Reservation.Enum
{
    public enum FieldType
    {
        [Display(Name = "متن")]
        Text=0,
        [Display(Name = "یک انتخابی")]
        SingleSelect=1,
        [Display(Name = "چند انتخابی")]
        MultiSelect=2
    }
}
