using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using shop.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static shop.Entities.Reservation.Enum.UserDeviceType;

namespace shop.ViewModels.Reservation
{
    public class RegisterUser
    {
        public int userId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RefererCode { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public bool IsProvider { get; set; }
        public bool IsAccept { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string natioalCode { get; set; }
        public string DisplayName { get; set; }
        public string PhotoFileName { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public DateTimeOffset? CreatedDateTime { get; set; }
        public DateTimeOffset? LastVisitDateTime { get; set; }
        public bool IsEmailPublic { get; set; }
        public string Location { set; get; }
        public string WalletPrice { set; get; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? LastLoggedIn { get; set; }
        public string SerialNumber { get; set; }
        public int? VerifyCode { get; set; }
        public DateTimeOffset? ExpireDateVerifyCode { get; set; }
        public bool? IsRegister { get; set; }

    }

    public class LoginViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

    }

    public class ChangeAppPasswordViewModel
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public int VerifyCode { get; set; }

    }

    public class NecessaryUserViewModel
    {
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string WalletPrice { set; get; }

    }

    public class UpdateUserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public bool? IsActive { get; set; }

    }

    public class UserSimpleViewModel 
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
    
    public class UpdateUserInfoViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NatioalCode { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class CooperationViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        /// <summary>
        /// عکس کد ملی
        /// </summary>
        public IFormFile Picture { get; set; }
        /// <summary>
        /// تلگرام
        /// </summary>
        public string Telegram { get; set; }
        /// <summary>
        /// اینستاگرام
        /// </summary>
        public string Instagram { get; set; }
        /// <summary>
        /// واتس اپ
        /// </summary>
        public string WhatsApp { get; set; }
        /// <summary>
        /// وب سایت
        /// </summary>
        public string WebSite { get; set; }
        /// <summary>
        /// ادرس
        /// </summary>
        public string Address { get; set; }
    }

    #region AppViewModel

    public class ChangeAppUserPasswordViewModel
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";

    }
    public class EditUserViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int? StateId { get; set; }
        public int? AreaId { get; set; }
        public int? CityId { get; set; }
    }
    public class UserInfoViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string natioalCode { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
    }

    #endregion


    #region api
    public class RegisterUserApi
    {
        public int userId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string RefererCode { get; set; }
        public int CityId { get; set; }
        public int AreaId { get; set; }
        public bool IsProvider { get; set; } = false;
    }
    public class FcmTokenUpdateApiViewModel
    {
        public string FcmToken { get; set; }
        public UserDeviceTypeApp AppType { get; set; } = UserDeviceTypeApp.customer;
    }
    #endregion

}
