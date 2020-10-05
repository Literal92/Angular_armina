using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using shop.Common.GuardToolkit;
using shop.Entities.Identity;
using shop.Services.Contracts.Identity;
using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace shop.Services.Token
{
    public class JwtTokensData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string RefreshTokenSerial { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }

    public interface ITokenFactoryService
    {
        Task<JwtTokensData> CreateJwtTokensAsync(User user, bool isPersist = false);
        string GetRefreshTokenSerial(string refreshTokenValue);
    }

    public class TokenFactoryService : ITokenFactoryService
    {
        private readonly ISecurityService _securityService;
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly IApplicationRoleManager _rolesService;
        private readonly IApplicationUserManager _userService;

        private readonly ILogger<TokenFactoryService> _logger;

        public TokenFactoryService(
            ISecurityService securityService,
            IApplicationRoleManager rolesService,
            IApplicationUserManager userService,

        IOptionsSnapshot<BearerTokensOptions> configuration,
            ILogger<TokenFactoryService> logger)
        {
            _securityService = securityService;
            _securityService.CheckArgumentIsNull(nameof(_securityService));

            _rolesService = rolesService;
            _rolesService.CheckArgumentIsNull(nameof(rolesService));

            _userService = userService;
            _userService.CheckArgumentIsNull(nameof(userService));

            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(configuration));

            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(logger));
        }


        public async Task<JwtTokensData> CreateJwtTokensAsync(User user, bool isPersist = false)
        {
            var (accessToken, claims) = await createAccessTokenAsync(user, isPersist);
            var (refreshTokenValue, refreshTokenSerial) = createRefreshToken();
            return new JwtTokensData
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                RefreshTokenSerial = refreshTokenSerial,
                Claims = claims
            };
        }

        private (string RefreshTokenValue, string RefreshTokenSerial) createRefreshToken()
        {
            var refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString().Replace("-", "");

            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.BearerTokens.Issuer, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.BearerTokens.Issuer),
                // for invalidation
                new Claim(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.BearerTokens.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.BearerTokens.Issuer,
                audience: _configuration.Value.BearerTokens.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.BearerTokens.RefreshTokenExpirationMinutes),
                signingCredentials: creds);
            var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return (refreshTokenValue, refreshTokenSerial);
        }

        public string GetRefreshTokenSerial(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return null;
            }

            ClaimsPrincipal decodedRefreshTokenPrincipal = null;
            try
            {
                decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                    refreshTokenValue,
                    new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.BearerTokens.Key)),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    },
                    out _
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to validate refreshTokenValue: `{refreshTokenValue}`.");
            }

            return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        }

        private async Task<(string AccessToken, IEnumerable<Claim> Claims)> createAccessTokenAsync(User user, bool isPersist = false)
        {
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.BearerTokens.Issuer, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.BearerTokens.Issuer),
               // new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                new Claim("DisplayName", user.DisplayName, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
                new Claim("UserType", user.UserType.ToString(), ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),

                // to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber,user.SerialNumber!=null ? user.SerialNumber:"", ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer),
               // new Claim("CityId",user.CityId!=null ? user.CityId.ToString() : "0", ClaimValueTypes.Integer32, _configuration.Value.BearerTokens.Issuer),
                //new Claim("AreaId",user.AreaId!=null ? user.AreaId.ToString() : "0", ClaimValueTypes.Integer32, _configuration.Value.BearerTokens.Issuer),
                // custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer)
            };

            // add roles
            var roles = (await _rolesService.FindUserRolesAsync(user.Id)).ToList();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer));
            }
            //// add RoleClaims to Claims
            var roleClaims = await _rolesService.GetClaimsAsync(roles);
            roleClaims.ForEach(s =>
            {
                claims.Add(new Claim("RoleClaims",
                   //JsonConvert.SerializeObject(new {item.Type,item.Value })
                   JsonConvert.SerializeObject(new { s.Type, s.Value })

                   , ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer));
            });
            ////
            //// add UserClaims to Claims
            var userClaims = (await _userService.GetClaimsAsync(user)).ToList();
            userClaims.ForEach(s =>
            {
                claims.Add(new Claim("UserClaims",
                   //JsonConvert.SerializeObject(new {item.Type,item.Value })
                   JsonConvert.SerializeObject(new { s.Type, s.Value })

                   , ClaimValueTypes.String, _configuration.Value.BearerTokens.Issuer));
            });
            ////
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.BearerTokens.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            if (isPersist)
            {
                var token = new JwtSecurityToken(
                              issuer: _configuration.Value.BearerTokens.Issuer,
                              audience: _configuration.Value.BearerTokens.Audience,
                              claims: claims,
                              notBefore: now,
                              expires: now.AddMonths(2),
                              signingCredentials: creds);

                return (new JwtSecurityTokenHandler().WriteToken(token), claims);
            }
            else
            {
                var token = new JwtSecurityToken(
                             issuer: _configuration.Value.BearerTokens.Issuer,
                             audience: _configuration.Value.BearerTokens.Audience,
                             claims: claims,
                             notBefore: now,
                             expires: now.AddMinutes(_configuration.Value.BearerTokens.AccessTokenExpirationMinutes),
                             signingCredentials: creds);

                return (new JwtSecurityTokenHandler().WriteToken(token), claims);
            }

        }
    }
}