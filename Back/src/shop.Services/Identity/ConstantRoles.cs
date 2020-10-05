using System.ComponentModel.DataAnnotations;

namespace shop.Services.Identity
{
    public static class ConstantRoles
    {
        public const string SuperAdmin = nameof(SuperAdmin);
      //  public const string Provider = nameof(Provider);
        public const string Client = nameof(Client);
        public const string Admin = nameof(Admin);
        /// <summary>
        /// مدیر فروشنده
        /// </summary>
        public const string ClientAdmin = nameof(ClientAdmin);
        /// <summary>
        ///مدیر مالی
        /// </summary>
        public const string AccountantAdmin = nameof(AccountantAdmin);
            /// <summary>
        ///مدیر سفارشات
            /// </summary>
        public const string OrderAdmin = nameof(OrderAdmin);
        ///<summary>
        /// مدیر محصولات
        ///</summary>
        public const string ProductAdmin = nameof(ProductAdmin);
        ///<summary>
        /// مدیر گزارشات
        ///</summary>
        public const string ReportAdmin = nameof(ReportAdmin);
    }
}