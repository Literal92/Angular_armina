﻿using System.ComponentModel.DataAnnotations;

namespace shop.ViewModels.Identity
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "(*)")]
        [EmailAddress(ErrorMessage = "لطفا آدرس ایمیل معتبری را وارد نمائید.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }
}