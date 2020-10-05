using shop.ViewModels.Identity.Settings;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shop.IocConfig;
using DNTCommon.Web.Core;
using shop.Common.WebToolkit;
using Microsoft.Extensions.Hosting;
using shop.ViewModels.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shop.Services.Token;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using shop.Extension;
using Microsoft.OpenApi.Models;
using shop.Services.Identity;
using DNTScheduler.Core;
using shop.ScheduledTasks;

namespace shop
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AutoMapperConfiguration.InitializeAutoMapper();
        }

        public IConfiguration Configuration { get; }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services)
        {
            // !! very important for use read appsetting in program
            services.Configure<SiteSettings>(options => Configuration.Bind(options));
            services.Configure<BearerTokensOptions>(options => Configuration.Bind(options));
            services.Configure<ApiSettingsOptions>(options => Configuration.Bind(options));
            services.Configure<PaymentSetting>(options => Configuration.GetSection("PaymentSetting").Bind(options));

            // Adds all of the ASP.NET Core Identity related services and configurations at once.
            services.AddCustomIdentityServices();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            #region Authentication
            services.AddAuthentication(options =>
              {
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

              })
               // .AddCookie(cfg => cfg.SlidingExpiration = true)
               .AddJwtBearer(cfg =>
               {
                   cfg.RequireHttpsMetadata = false;
                   cfg.SaveToken = true;
                   cfg.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = Configuration["BearerTokens:Issuer"], // site that makes the token
                       ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                       ValidAudience = Configuration["BearerTokens:Audience"], // site that consumes the token
                       ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["BearerTokens:Key"])),
                       ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                       ValidateLifetime = true, // validate the expiration
                       ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                   };
                   cfg.Events = new JwtBearerEvents
                   {
                       OnAuthenticationFailed = context =>
                       {
                           var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                           logger.LogError("Authentication failed.", context.Exception);
                           return Task.CompletedTask;
                       },
                       OnTokenValidated = context =>
                       {
                           var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                           return tokenValidatorService.ValidateAsync(context);
                       },
                       OnMessageReceived = context =>
                       {
                           return Task.CompletedTask;
                       },
                       OnChallenge = context =>
                       {
                           var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                           logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                           return Task.CompletedTask;
                       }
                   };
               });
            #endregion

            #region CorsOrigin
            // more info :https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder
                    // Note: The URL must not contain a trailing slash (/). If the URL terminates with /, the comparison returns false and no header is returned.
                    //.WithOrigins("http://example.com",
                    //             "http://www.contoso.com")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });

            #endregion


            #region swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = "Client Api",
                    Version = "v1.0"
                });
                c.SwaggerDoc("v2.0", new OpenApiInfo { Title = "Admin Api", Version = "v2.0" });
                c.SwaggerDoc("v3.0",new OpenApiInfo { Title = "Common Api", Version = "v3.0" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                 {
                   new OpenApiSecurityScheme
                   {
                     Reference = new OpenApiReference
                     {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Bearer"
                     }
                    },
                    new string[] { }
                  }
                });
            });
            #endregion


            #region ApiVersioning

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                //options.ApiVersionReader = new QueryStringApiVersionReader("version");
                //options.ApiVersionReader = new HeaderApiVersionReader("x-apiversion");
                // not work(enable both of them)
                ////var multiVersionReader = new QueryStringOrHeaderApiVersionReader("version");
                ////multiVersionReader.HeaderNames.Add("x-apiversion");
                ////options.ApiVersionReader = multiVersionReader;
                //// or
                ////options.ApiVersionReader = new QueryStringOrHeaderApiVersionReader("x-api-version");

            });


            #endregion


            #region Policy
            //More Info :https://stackoverflow.com/questions/35609632/asp-net-5-authorize-against-two-or-more-policies

            services.AddAuthorization(options =>
            {
                //var userPrincipal = context.Principal;

                //var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                //if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
                //{
                //    context.Fail("This is not our issued token. It has no claims.");
                //    return;
                //}

                //var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);


                options.AddPolicy(ConstantRoles.Admin,
                    policy => policy.RequireAssertion(context =>
                              context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                              context.User.IsInRole(ConstantRoles.Admin)));

                options.AddPolicy(ConstantRoles.Client,
                 policy => policy.RequireAssertion(context =>
                          context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                          context.User.IsInRole(ConstantRoles.Admin) ||
                          context.User.IsInRole(ConstantRoles.Client)));

                options.AddPolicy(ConstantPolicies.AccountantAdmin,
                   policy => policy.RequireAssertion(context =>
                         context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                         context.User.IsInRole(ConstantRoles.AccountantAdmin)));

                options.AddPolicy(ConstantPolicies.OrderAdmin,
                                   policy => policy.RequireAssertion(context =>
                                         context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                                         context.User.IsInRole(ConstantRoles.OrderAdmin)));

                options.AddPolicy(ConstantPolicies.ProductAdmin,
                                  policy => policy.RequireAssertion(context =>
                                        context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                                        context.User.IsInRole(ConstantRoles.ProductAdmin)));

                options.AddPolicy(ConstantPolicies.OrderView,
                   policy => policy.RequireAssertion(context =>
                         context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                         context.User.IsInRole(ConstantRoles.AccountantAdmin)||
                         context.User.IsInRole(ConstantRoles.OrderAdmin)||
                         context.User.IsInRole(ConstantRoles.ReportAdmin)));

                options.AddPolicy(ConstantPolicies.ReportAdmin,
                  policy => policy.RequireAssertion(context =>
                        context.User.IsInRole(ConstantRoles.SuperAdmin) ||
                        context.User.IsInRole(ConstantRoles.ReportAdmin)));

                //options.AddPolicy("Client",
                //  policy => policy.RequireRole(Client));
            });
            #endregion

            #region Https
            //services.AddHttpsRedirection(options =>
            //{
            //    options.HttpsPort = 443;
            //});
            #endregion

            services.AddControllers().AddNewtonsoftJson(); // add Newtonsoftjson
            services.AddControllersWithViews();
            services.AddMvc(options => options.UseYeKeModelBinder())
            .AddNewtonsoftJson(options => // add Newtonsoftjson
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
            });



            services.AddDNTCommonWeb();
            
            services.AddCloudscribePagination();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });


            #region Scheduler

            services.AddDNTScheduler(options =>
            {
                options.AddScheduledTask<DoCancelReserveTask>(
                    runAt: utcNow =>
                    {
                        var now = utcNow.AddHours(3.5);
                        return now.Minute % 60 == 0 && now.Second == 1;
                       // return now.Minute % 60 == 0 && now.Second == 1;
                    },
                    order: 1);
            });
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();

                app.ErrorHandlingMiddleware();

            }
            else
            {
                app.ErrorHandlingMiddleware();
                app.UseHsts();
                //app.UseExceptionHandler("/error");
            }

            app.UseCors(MyAllowSpecificOrigins);

            app.UseHttpsRedirection();

            app.UseContentSecurityPolicy();
            app.UseDNTScheduler();
            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API");
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Client Api");
                c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Admin Api");
                c.SwaggerEndpoint("/swagger/v3.0/swagger.json", "Common Api");


            });
            //    app.UseSwaggerUI(c =>
            //    {
            //        c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Reservation Api v1.0");
            //        c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Reservation Api v2.0");
            //    //c.SwaggerEndpoint("/swagger/v3/swagger.json", "Reservation Api v3");
            //    //c.SwaggerEndpoint("/swagger/v4/swagger.json", "Reservation Api v4");

            //    // c.RoutePrefix = string.Empty;


            //});
            #endregion

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller=Account}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

            });

            #region Routing Spa
            // before install  install Microsoft.AspNetCore.SpaServices.Extensions

            app.Map("/admin", appSearch => appSearch.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot/admin";
                spa.Options.DefaultPage = "/admin/index.html";

            }));
            app.Map("", appSearch => appSearch.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
                spa.Options.DefaultPage = "/index.html";

            }));
            #endregion

        }
    }
}