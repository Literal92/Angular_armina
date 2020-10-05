
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class FieldViewModel : BaseDto<FieldViewModel, Field>
    {
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public FieldType FieldType { get; set; }
        public int Order { get; set; }

        #region Relations
        public virtual ICollection<ProductOptionViewModel> ProductOptions { get; set; }
        #endregion
    }

    public class FieldGroupByViewModel : BaseDto<FieldGroupByViewModel, Field>
    {
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public FieldType FieldType { get; set; }
        public int Order { get; set; }

        #region Relations
        public virtual ICollection<ProductOptionWithColorsViewModel> ProductOptions { get; set; }
        #endregion
    }

    public class FieldForBasketViewModel : BaseDto<FieldForBasketViewModel, Field>
    {
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public FieldType FieldType { get; set; }
        public int Order { get; set; }

        #region Relations
        public virtual ICollection<ProductOptionWithColorsViewModel> ProductOptions { get; set; }
        #endregion
    }

}