using AC.SD.Core.Configuration;
using Infrastructure;
using Application;
using Blazored.Toast;
using AC.ShippingDocument.Reporting;
using Core.Services;
using SW.Portal.Solutions.Models;
using AC.SD.Core.Services;
using Blazored.SessionStorage;
using SW.Portal.Solutions.Services;
namespace SW.Portal.Solutions.ServerSide
{
    partial class Startup
    {
        private IWebHostEnvironment env;

        //public IConfiguration Configuration { get; }
        public override string EnvironmentName => "ServerSide";
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
#if DEBUG
            bool detailedErrors = true;
#else
            bool detailedErrors = Configuration.GetValue("detailedErrors", false);
#endif
            //services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = detailedErrors);
            services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                options.DetailedErrors = true;
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(10);
                options.DisconnectedCircuitMaxRetained = 0;
            });
            services.AddBlazoredToast();
            services.AddDevExpressServerSideBlazorReportViewer();
            
            services.AddControllersWithViews();           

            services.AddSingleton<IDemoVersion, DemoVersion>(x =>
            {
                string customVersion = Configuration.GetValue<string>("dxversion");
                if (!string.IsNullOrEmpty(customVersion))
                    customVersion = " " + customVersion.TrimStart();
                var dxVersion = new Version(AssemblyInfo.Version);
                return new DemoVersion(new Version(dxVersion.Major, dxVersion.Minor, dxVersion.Build) + customVersion);
            });
            services.AddTransient<HttpClient>(serviceProvider => serviceProvider.GetService<IHttpClientFactory>().CreateClient());            
            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddHttpContextAccessor();

            ////Enable CORS
            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //});
            //services.Configure<RequestBodyOptions>(options =>
            //{
            //    options.MinRequestBodyDataRate = 1024; // 1 kilobyte per second
            //});

            services.AddRazorComponents(); // Ensure this is called if required
            services.AddDevExpressBlazor(); // Register DevExpress Blazor services
            services.AddOptions();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddBlazoredSessionStorage();
            services.AddSingleton<DemoConfiguration>();
            services.AddScoped<DemoThemeService>();
            services.AddScoped<IDemoStaticResourceService, DemoStaticResourceService>();
            services.AddDevExpressServerSideBlazorReportViewer();
            services.AddScoped<ClipboardService>();
            services.AddScoped<FirebaseMessagingService>();


            //services.AddControllersWithViews();
            services.AddHttpClient();

            // Inject IConfiguration to access configuration settings
            var configuration = context.Configuration;

            // Configure strongly typed settings objects
            var appSettingsSection = configuration.GetSection("FcmNotification");
            services.Configure<FcmNotificationSetting>(appSettingsSection);
            services.Configure<RequestBodyOptions>(options =>
            {
                options.MinRequestBodyDataRate = 1024; // 1 kilobyte per second
            });

        }
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");                
                app.UseHsts();
            }
            app.UseCors("AllowOrigin");
            app.UseStaticFiles();            
            //app.UseWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(); // Map Razor Pages
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
            });

            AppDependencyResolver.Init(app.ApplicationServices);
        }
        public override void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureAppConfiguration(((context, configurationBuilder) =>
            //{
            //    configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            //    {
            //        ["DataSourcesFolder"] = Path.Combine(System.AppContext.BaseDirectory, "DataSources")
            //    });
            //}));

            builder.UseIISIntegration();
            builder.UseIIS();

            base.Configure(builder);
            ReportingHostingStartup.Configure(builder);
        }
    }
}
