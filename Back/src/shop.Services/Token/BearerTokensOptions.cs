namespace shop.Services.Token
{
 
    public class BearerTokensOptions
    {
        public BearerTokens BearerTokens { get; set; }
    }
    public class BearerTokens
    {
        public string Key { set; get; }
        public string Issuer { set; get; }
        public string Audience { set; get; }
        public int AccessTokenExpirationMinutes { set; get; }
        public int RefreshTokenExpirationMinutes { set; get; }
        public bool AllowMultipleLoginsFromTheSameUser { set; get; }
        public bool AllowSignoutAllUserActiveClients { set; get; }
    }
}