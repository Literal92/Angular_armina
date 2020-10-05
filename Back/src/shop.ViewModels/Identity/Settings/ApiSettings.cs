namespace shop.ViewModels.Identity.Settings
{
    public class ApiSettings
    {
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string RefreshTokenPath { get; set; }
        public string AccessTokenObjectKey { get; set; }
        public string RefreshTokenObjectKey { get; set; }
        public string AdminRoleName { get; set; }
        public int MinLengthPass { get; set; }
    }
    public class ApiSettingsOptions
    {
        public ApiSettings ApiSettings { get; set; }
    }
}