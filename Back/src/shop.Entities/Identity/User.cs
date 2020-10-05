using shop.Entities.AuditableEntity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using shop.Entities.Reservation;
using shop.Entities.Identity.Enum;
using static shop.Entities.Identity.Enum.UserTypeEnum;

namespace shop.Entities.Identity
{
    public class User : IdentityUser<int>, IAuditableEntity
    {
        public User()
        {
            UserUsedPasswords = new HashSet<UserUsedPassword>();
            UserTokens = new HashSet<UserToken>();
            CustomUserTokens = new HashSet<CustomUserToken>();
            UserDevices = new HashSet<UserDevice>();
        }
        [StringLength(450)]
        public string Mobile { get; set; }

        [StringLength(450)]
        public string FirstName { get; set; }

        [StringLength(450)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string natioalCode { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                var displayName = $"{FirstName} {LastName}";
                return string.IsNullOrWhiteSpace(displayName) ? UserName : displayName;
            }
        }

        [StringLength(450)]
        public string PhotoFileName { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? LastVisitDateTime { get; set; }
        public bool IsEmailPublic { get; set; }

        //کدمعرف کاربر
        public string RefererCode { set; get; }

        public string Location { set; get; }
        public int? WalletPrice { set; get; }
        public int? Score { set; get; }

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoggedIn { get; set; }
        /// <summary>
        /// every time the user changes his Password,
        /// or an admin changes his Roles or stat/IsActive,
        /// create a new `SerialNumber` GUID and store it in the DB.
        /// </summary>
        public string SerialNumber { get; set; }
        public bool FirstReserve { get; set; }
        public int? VerifyCode { get; set; }

        public DateTime? ExpireDateVerifyCode { get; set; }
        public bool? IsRegister { get; set; }

        public bool? IsRequestRestPassword { get; set; }
        public UserType UserType { get; set; }
        public double Rate { get; set; }
        public int RateCount { get; set; }
        // درصد تخفیف
        public int? PercentDiscount { get; set; }

        #region درخواست همکاری- coopertion
        /// <summary>
        /// درخواست همکاری
        /// </summary>
        public bool IsRequest { get; set; }
        /// <summary>
        /// عکس کد ملی
        /// </summary>
        public string Picture { get; set; }
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
        #endregion


        public virtual ICollection<UserUsedPassword> UserUsedPasswords { get; set; }
            
        public virtual ICollection<UserToken> UserTokens { get; set; }
        public virtual ICollection<CustomUserToken> CustomUserTokens { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }


        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual ICollection<UserDevice> UserDevices { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}