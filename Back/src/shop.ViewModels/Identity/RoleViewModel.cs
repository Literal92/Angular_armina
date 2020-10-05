using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shop.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace shop.ViewModels.Identity
{
    public class RoleViewModel
    {
        [HiddenInput]
        public string Id { set; get; }

        [Required(ErrorMessage = "(*)")]
        [Display(Name = "نام نقش")]
        public string Name { set; get; }
    }


    public class RoleCustomeViewModel
    {
        public int Id { set; get; }

        [Required(ErrorMessage = "(*)")]
        [Display(Name = "نام نقش")]
        public string Name { set; get; }

        [Display(Name = "نام فارسی")]
        public string Description { set; get; }      
    }
}