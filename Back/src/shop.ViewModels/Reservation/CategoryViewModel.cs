
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class CategoryViewModel : BaseDto<CategoryViewModel, Category>
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Path { get; set; } = "";
        public string Thumbnail { get; set; }

        public int Order { get; set; }
        //public int Count { get; set; }


        #region Relations

        public int? ParentId { get; set; }

        public CategoryViewModel Parent { get; set; }
        public List<CategoryViewModel> SubCategory { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        #endregion

    }
    public class CategoryTreeAppViewModel : BaseDto<CategoryTreeAppViewModel, Category>
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Thumbnail { get; set; } = "";

        public int Order { get; set; }

        #region Relations


        public int? ParentId { get; set; }
        public CategoryTreeAppViewModel Parent { get; set; }
        public ICollection<CategoryTreeAppViewModel> SubCategory { get; set; }      

        #endregion

    }

   

}