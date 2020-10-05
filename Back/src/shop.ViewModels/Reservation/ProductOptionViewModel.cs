
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class ProductOptionViewModel : BaseDto<ProductOptionViewModel, ProductOption>
    {
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        public int Order { get; set; }

        #region Relations
        public int ProductId { get; set; }
        public ProductViewModel Product { get; set; }
        public int FieldId { get; set; }
        public virtual FieldViewModel Field { get; set; }
        public virtual ICollection<OptionColorViewModel> OptionColors { get; set; }

        #endregion

    }


    public class ProductOptionSimpleViewModel : BaseDto<ProductOptionSimpleViewModel, ProductOption>
    {
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        public int Order { get; set; }

    }
    public class ProductOptionWithColorsViewModel : BaseDto<ProductOptionSimpleViewModel, ProductOption>
    {
        public ProductOptionWithColorsViewModel()
        {
            OptionColors = new List<OptionColorSimpleViewModel>();
        }
        public string Title { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        public int Order { get; set; }
        #region Relations
        public List<OptionColorSimpleViewModel> OptionColors { get; set; }
        #endregion

    }


}