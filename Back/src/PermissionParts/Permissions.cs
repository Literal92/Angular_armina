using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PermissionParts.Enum
{
    // more info : https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
    public enum Permissions
    {
        #region User
        [Display(GroupName = "User", Name = "Get", Description = "دریافت لیست کاربران سیستمی")]
        UserRead = 0,
        [Display(GroupName = "User", Name = "Create", Description = "ایجاد کاربر سیستمی")]
        UserCreate = 1,
        [Display(GroupName = "User", Name = "Update", Description = "ویرایش کاربر سیستمی")]
        UserUpdate = 2,
        [Display(GroupName = "User", Name = "ActiveUser", Description = "فعال/غیر فعال کردن کاربران سیستمی")]
        UserDelete = 3,
        [Display(GroupName = "User", Name = "ChangePass", Description = "تغییر پسورد کاربران سیستمی")]
        UserChangePass = 4,
        #endregion

        #region Permission
        [Display(GroupName = "Permission", Name = "Create", Description = "اعمال سطح دسترسی")]
        PermissionCreate = 5,
        #endregion
 
        #region Organization
        [Display(GroupName = "Organization", Name = "Get", Description = "مشاهده لیست سازمان ها")]
        OrganizationRead = 6,
        [Display(GroupName = "Organization", Name = "Create", Description = "ایجاد سازمان")]
        OrganizationCreate = 7,
        [Display(GroupName = "Organization", Name = "Update", Description = "ویرایش سازمان")]
        OrganizationUpdate=8,
        [Display(GroupName = "Organization", Name = "Delete", Description = "حذف سازمان")]
        OrganizationDelete = 9,
        #endregion
        //#region Provider
        //[Display(GroupName = "Provider", Name = "Get", Description = "مشاهده لیست پروایدر ها")]
        //ProviderRead = 10,
        //[Display(GroupName = "Provider", Name = "Create", Description = "ایجاد پروایدر")]
        //ProviderCreate = 11,
        //[Display(GroupName = "Provider", Name = "Update", Description = "ویرایش پروایدر")]
        //ProviderUpdate = 12,
        //[Display(GroupName = "Provider", Name = "Delete", Description = "حذف پروایدر")]
        //ProviderDelete = 13,
        //#endregion


    }
}
