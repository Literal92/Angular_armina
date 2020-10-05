using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace shop.ViewModels.Reservation

{
    public class UserPostViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public int ? CityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NatioalCode { get; set; }
        public string PhotoFileName { get; set; }
    }
    public class UserGetViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string NatioalCode { get; set; }
        public string PhotoFileName { get; set; }
        public bool IsActive { get; set; }
        #region Relation
        public int? CityId { get; set; } 
        public string CityTitle { get; set; }
     //   public ICollection<UserClaim> Claims { get; set; }
        #endregion
    }

}
