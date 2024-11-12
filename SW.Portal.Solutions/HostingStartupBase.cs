using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Blazored.Toast;
using AC.SD.Core.Services;
using Application.Common.Helper;
using Blazored.LocalStorage;
using Google.Cloud.Firestore;
using SW.Portal.Solutions.Services;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;


[assembly: HostingStartup(typeof(SW.Portal.Solutions.ServerSide.Startup))]

namespace SW.Portal.Solutions.ServerSide
{
    partial class Startup : HostingStartupBase { }

    abstract class HostingStartupBase : IHostingStartup
    {
        protected IConfiguration Configuration { get; private set; }

        public abstract string EnvironmentName { get; }

        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment env);
        public abstract void ConfigureServices(WebHostBuilderContext context, IServiceCollection services);


        public virtual void Configure(IWebHostBuilder builder)
        {
            builder.UseEnvironment(EnvironmentName);
            builder.UseStaticWebAssets();
            builder.ConfigureServices(ConfigureServices);
            builder.Configure(ConfigureApp);
            builder.UseIISIntegration();
            builder.UseIIS();

            void ConfigureApp(WebHostBuilderContext context, IApplicationBuilder app)
            {
                string pathBase = Configuration.GetValue<string>("pathbase");
                if (!string.IsNullOrEmpty(pathBase))
                {
                    string pathString = pathBase.StartsWith('/') ? pathBase : "/" + pathBase;
                    app.UsePathBase(pathString);
                }

                app.UseRequestLocalization(new RequestLocalizationOptions().SetDefaultCulture("en-US"));
                app.UseAuthentication();
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCors("AllowOrigin");

                app.UseResponseCompression();
                app.UseRouting();

                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".razor"] = "text/plain";
                provider.Mappings[".cshtml"] = "text/plain";
                provider.Mappings[".cs"] = "text/plain";
                app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapFallbackToPage("/_Host");

                });
                // Add your dashboard route mapping here

                Configure(app, context.HostingEnvironment);
            };


            void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            {
                Configuration = services.BuildServiceProvider().GetService<IConfiguration>();


                // Add the following block to add the DashboardConfigurator service
                IConfiguration configuration = context.Configuration;
                IFileProvider fileProvider = context.HostingEnvironment.ContentRootFileProvider;

                services.AddHttpClient<HttpClient>(ConfigureHttpClient);
                services.AddBlazoredLocalStorage();
                this.ConfigureServices(context, services);
                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                });
                services.Configure<BrotliCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.SmallestSize;
                });
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.Cookie.Name = "sunwardpharma";
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.LoginPath = new PathString("/login");
                   options.LogoutPath = new PathString("/logout");
               });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("RequireLoggedIn", policy =>
                        policy.RequireAuthenticatedUser());
                });

                services.AddScoped<FirebaseMessagingService>();
                services.AddScoped<ToastService>();
                services.AddScoped<IFirebaseSync, FirebaseSync>();

                services.AddSingleton<IJobFactory, SingletonJobFactory>();
                services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                // Configure Firestore
                ConfigureFirestore(services);

                services.AddScoped<Helper>();
                services.AddServerSideBlazor();
                services.AddBlazoredToast();
                services.AddRazorPages();
                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.MimeTypes = new[] {
                            "text/plain",
                            "text/css",
                            "application/javascript",
                            "text/html",
                            "application/xml",
                            "text/xml",
                            "application/json",
                            "text/json"
                        };
                });
                services.AddControllers().AddJsonOptions(ConfigureJsonOptions);

                //Enable CORS
                services.AddCors(c =>
                {
                    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                });



                static void ConfigureHttpClient(HttpClient httpClient)
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                };

                static void ConfigureJsonOptions(JsonOptions options)
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                };


            }
        }

        void ConfigureFirestore(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "firebase-credentials.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            FirestoreDb firestoreDb = FirestoreDb.Create("novatonotify"); // Replace with your Firestore project ID
            services.AddSingleton(_ => firestoreDb);
        }


        void IHostingStartup.Configure(IWebHostBuilder builder)
        {
            Configure(builder);
        }
    }
}
