
using Microsoft.AspNetCore.Http;
using shop.Entities.Reservation;
using shop.Entities.Reservation.Enum;
using shop.ViewModels;
using System.Collections.Generic;

namespace shop.ViewModels.Reservation
{
    public class ProductViewModel : BaseDto<ProductViewModel, Product>
    {
        public string Title { get; set; }
        public string Pic { get; set; }
        public string[] Gallery { get; set; }
        public string Thumbnail { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public bool? IsPublish { get; set; }
        // کد محصول
        public string Code { get; set; }
        #region Relations
        public int CategoryId { get; set; }
        public virtual CategoryViewModel Category { get; set; }

        public int? ParentId { get; set; }
        public virtual ProductViewModel Parent { get; set; }
        public virtual ICollection<ProductViewModel> ChildProduct { get; set; }
        public virtual ICollection<ProductOptionViewModel> ProductOptions { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }

        #endregion
    }
    public class ProductDetailsViewModel : BaseDto<ProductDetailsViewModel, Product>
    {
        public string Title { get; set; }
        public string Pic { get; set; }
        public string Thumbnail { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        // کد محصول
        public string Code { get; set; }
        #region Relations
        public int CategoryId { get; set; }
        public virtual CategoryViewModel Category { get; set; }
        public virtual ICollection<FieldGroupByViewModel> Fields { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        #endregion
    }
    public class ProductForBasketViewModel : BaseDto<ProductForBasketViewModel, Product>
    {
        public string Title { get; set; }
        public string Pic { get; set; }
        public string Thumbnail { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        // کد محصول
        public string Code { get; set; }
    }



}