using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shop.Entities.Identity;
using shop.Entities.Reservation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shop.ViewModels.Identity
{
    public class UserViewModel 
    {
      


        //[Remote("ValidateUsername", "UserProfile",
        //   AdditionalFields = nameof(Email) + "," + ViewModelConstants.AntiForgeryToken + "," + nameof(Pid),
        //   HttpMethod = "POST")]
        [Required(ErrorMessage = "(*)")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "شماره همراه")]
        public string Mobile { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "(*)")]
        [StringLength(450)]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "(*)")]
        [StringLength(450)]
        public string LastName { get; set; }

        //[Remote("ValidateUsername", "UserProfile",
        //    AdditionalFields = nameof(UserName) + "," + ViewModelConstants.AntiForgeryToken + "," + nameof(Pid),
        //    HttpMethod = "POST")]
        [Required(ErrorMessage = "(*)")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبری را وارد نمائید.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(maximumLength: 1000, ErrorMessage = "حداکثر طول آدرس تصویر 1000 حرف است.")]
        public string PhotoFileName { set; get; }

        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }

        public string BirthDay { set; get; }
        public double Rate { get; set; }
        public int RateCount { get; set; }
    }
}
