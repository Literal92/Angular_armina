namespace shop.Services.Identity
{
    public static class ConstantPolicies
    {
        public const string DynamicPermission = nameof(DynamicPermission);
        public const string DynamicPermissionClaimType = nameof(DynamicPermission);
        /// <summary>
        /// مشاهده سفارشات
        /// </summary>
        public const string OrderView = nameof(OrderView);
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