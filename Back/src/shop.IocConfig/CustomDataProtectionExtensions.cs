
using shop.Services.Identity;
using shop.ViewModels.Identity.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace shop.IocConfig
{
    // more Info : https://www.componentspace.com/Forums/29/Troubleshooting-Loading-X509-Certificates
    //public static class CustomDataProtectionExtensions
    //{
    //    public static IServiceCollection AddCustomDataProtection(
    //        this IServiceCollection services, SiteSettings siteSettings)
    //    {
    //        services.AddSingleton<IXmlRepository, DataProtectionKeyService>();
    //        services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
    //        {
    //            return new ConfigureOptions<KeyManagementOptions>(options =>
    //            {
    //                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    //                using (var scope = scopeFactory.CreateScope())
    //                {
    //                    options.XmlRepository = scope.ServiceProvider.GetRequiredService<IXmlRepository>();
    //                }
    //            });
    //        });

    //        var certificate = loadCertificateFromFile(siteSettings);
    //        services
    //            .AddDataProtection()
    //            .SetDefaultKeyLifetime(siteSettings.DataProtectionOptions.DataProtectionKeyLifetime)
    //            .SetApplicationName(siteSettings.DataProtectionOptions.ApplicationName)
    //            .ProtectKeysWithCertificate(certificate);

    //        return services;
    //    }

    //    private static X509Certificate2 loadCertificateFromFile(SiteSettings siteSettings)
    //    {
    //        // NOTE:
    //        // You should check out the identity of your application pool and make sure
    //        // that the `Load user profile` option is turned on, otherwise the crypto susbsystem won't work.

    //        var certificate = siteSettings.DataProtectionX509Certificate;
    //        var fileName = Path.Combine(ServerInfo.GetAppDataFolderPath(), certificate.FileName);

    //        // For decryption the certificate must be in the certificate store. It's a limitation of how EncryptedXml works.
    //        using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
    //        {
    //            store.Open(OpenFlags.ReadWrite);
    //            store.Add(new X509Certificate2(fileName, certificate.Password, X509KeyStorageFlags.Exportable));
    //        }

    //        return new X509Certificate2(
    //            fileName,
    //            certificate.Password,
    //            keyStorageFlags: X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
    //                             | X509KeyStorageFlags.Exportable);
    //    }
    //}


    public static class CustomDataProtectionExtensions
    {
        public static IServiceCollection AddCustomDataProtection(
            this IServiceCollection services, SiteSettings siteSettings)
        {
            services.AddSingleton<IXmlRepository, DataProtectionKeyService>();
            services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(serviceProvider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                    using (var scope = scopeFactory.CreateScope())
                    {
                        options.XmlRepository = scope.ServiceProvider.GetService<IXmlRepository>();
                    }
                });
            });
            services
                .AddDataProtection()
                .SetDefaultKeyLifetime(siteSettings.CookieOptions.ExpireTimeSpan)
                .SetApplicationName(siteSettings.CookieOptions.CookieName)
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

            return services;
        }
    }
}