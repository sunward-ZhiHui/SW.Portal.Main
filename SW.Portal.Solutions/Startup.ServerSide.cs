using System;
using System.IO;
using System.Net.Http;
using AC.SD.Core.Configuration;
using AC.SD.Core.DataProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;
using AC.SD.Core.Services;
using Infrastructure;
using Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blazored.Toast;
using AC.ShippingDocument.Reporting;
using Application.Constant;
using SW.Portal.Solutions.Hubs;
using Core.FCM;
using Core.Services;
using SW.Portal.Solutions.Models;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using SW.Portal.Solutions.Code;
using AC.SD.Core;
using DevExpress.CodeParser;
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
            //services.AddTransient<RealtimeService>();
            //services.AddTransient<EmailAutoRefresh>();

            //var keys = WebPush.VapidHelper.GenerateVapidKeys();
            //System.Diagnostics.Debug.WriteLine(keys.PrivateKey);
            //System.Diagnostics.Debug.WriteLine(keys.PublicKey);

            services.AddControllersWithViews();
            services.AddDemoServices(Configuration, env);

            services.AddSingleton<IDemoVersion, DemoVersion>(x =>
            {
                string customVersion = Configuration.GetValue<string>("dxversion");
                if (!string.IsNullOrEmpty(customVersion))
                    customVersion = " " + customVersion.TrimStart();
                var dxVersion = new Version(AssemblyInfo.Version);
                return new DemoVersion(new Version(dxVersion.Major, dxVersion.Minor, dxVersion.Build) + customVersion);
            });
            services.AddTransient<HttpClient>(serviceProvider => serviceProvider.GetService<IHttpClientFactory>().CreateClient());
            //services.AddInfrastructureServices(Configuration);
            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddHttpContextAccessor();

            //services.AddTransient<IFcm>(s => new FcmBuilder()
            //   .WithApiKey("Your_API_key")
            //   .GetFcm()
            // );


            //Enable CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.Configure<RequestBodyOptions>(options =>
            {
                options.MinRequestBodyDataRate = 1024; // 1 kilobyte per second
            });
            //services.AddScoped<IContosoRetailDataProvider, ContosoRetailDataProvider>();
            //services.AddScoped<IRentInfoDataProvider, RentInfoDataProvider>();

            //services.AddDbContextFactory<NorthwindContext>(opt => {
            //    var connectionString = ConnectionStringUtils.GetNorthwindConnectionString(context.Configuration);
            //    if(!string.IsNullOrEmpty(connectionString))
            //        opt.UseSqlServer(connectionString);
            //    else
            //        opt.UseSqlite(ConnectionStringUtils.GetNorthwindSqliteConnectionString(context.Configuration));
            //});
            //services.AddDbContextFactory<IssuesContext>(opt => {
            //    var connectionString = ConnectionStringUtils.GetIssuesConnectionString(context.Configuration);
            //    if(!string.IsNullOrEmpty(connectionString))
            //        opt.UseSqlServer(connectionString);
            //    else
            //        opt.UseSqlite(ConnectionStringUtils.GetIssuesSqliteConnectionString(context.Configuration));
            //});
            //services.AddDbContextFactory<WorldcitiesContext>(opt => {
            //    var connectionString = ConnectionStringUtils.GetWorlcitiesConnectionString(context.Configuration);
            //    if(!string.IsNullOrEmpty(connectionString))
            //        opt.UseSqlServer(connectionString);
            //    else
            //        opt.UseSqlite(ConnectionStringUtils.GetWorlcitiesSqliteConnectionString(context.Configuration));
            //});
            //services.AddDbContextFactory<RentInfoContext>(opt => {
            //    var connectionString = ConnectionStringUtils.GetGridLargeDataConnectionString(context.Configuration);
            //    if(!string.IsNullOrEmpty(connectionString))
            //        opt.UseSqlServer(connectionString);
            //});
            //services.AddDbContextFactory<ContosoRetailContext>(opt => {
            //    var connectionString = ConnectionStringUtils.GetPivotGridLargeDataConnectionString(context.Configuration);
            //    if(!string.IsNullOrEmpty(connectionString))
            //        opt.UseSqlServer(connectionString);
            //});
            //services.AddSingleton<IStockQuoteService, StockQuoteService>();
            //services.AddHostedService<StockQuoteChangeTimerService>(
            //    provider => new StockQuoteChangeTimerService((StockQuoteService)provider.GetRequiredService<IStockQuoteService>())
            //);

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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("AllowOrigin");
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions {
            //    ServeUnknownFileTypes = true
            //});
            app.UseWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                //endpoints.MapHub<NotificationHub>(ApplicationConstants.SignalR.HubUrl);
            });

            AppDependencyResolver.Init(app.ApplicationServices);
        }
        public override void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(((context, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["DataSourcesFolder"] = Path.Combine(System.AppContext.BaseDirectory, "DataSources")
                });
            }));

            builder.UseIISIntegration();
            builder.UseIIS();

            base.Configure(builder);
            ReportingHostingStartup.Configure(builder);
        }
    }
}
