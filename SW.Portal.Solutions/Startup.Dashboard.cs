using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.DashboardWeb;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
namespace SW.Portal.Solutions
{

    public static class DashboardConfig
    {
        public static void RegisterDashboard(IServiceCollection services, IConfiguration configuration, IFileProvider fileProvider, IHttpContextAccessor httpContextAccessor)
        {

            // string BaseDirectory = System.AppContext.BaseDirectory;
            // var dashboardFolder = Path.Combine(BaseDirectory, "Dashboards");
            //  services.AddControllers();
            //  services.AddControllersWithViews();
            // services.AddRazorComponents()
            // .AddInteractiveServerComponents();
            // services.AddRazorPages();
            //services.AddServerSideBlazor();
            // services.AddMvc();
            //services.AddDevExpressControls();


            services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
            {
                DashboardConfigurator configurator = new DashboardConfigurator();

                // === Dashboard storage selection logic ===
                var defaultDashboardPath = fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath;
                var spcDashboardPath = fileProvider.GetFileInfo("Data/SPCDashboards").PhysicalPath;

                // Example: conditionally use one or the other
                var httpContext = httpContextAccessor.HttpContext;
                var dashboardType = httpContext?.Request?.Headers["DashboardType"].FirstOrDefault();

                if (!string.IsNullOrEmpty(dashboardType) && dashboardType.Equals("SPC", StringComparison.OrdinalIgnoreCase))
                {
                    configurator.SetDashboardStorage(new DashboardFileStorage(spcDashboardPath));
                    Console.WriteLine("Using SPC Dashboard path: " + spcDashboardPath);
                }
                else
                {
                    configurator.SetDashboardStorage(new DashboardFileStorage(defaultDashboardPath));
                    Console.WriteLine("Using Default Dashboard path: " + defaultDashboardPath);
                }

                // === Other config ===
                configurator.SetDataSourceStorage(new DataSourceInMemoryStorage());
                configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

                // JSON data sources registration
                var jsonStorage = new DataSourceInMemoryStorage();

                var jsonFiles = new[]
                {
                     ("spc_trending", "wwwroot/data/SPCTrendingData.json"),
                     ("spc_finished_product", "wwwroot/data/SPCFinishedProductData.json")
                };

                foreach (var (key, relativePath) in jsonFiles)
                {
                    var jsonPath = fileProvider.GetFileInfo(relativePath).PhysicalPath;
                    if (File.Exists(jsonPath))
                    {
                        var jsonText = File.ReadAllText(jsonPath);
                        var jsonDataSource = new DashboardJsonDataSource(key.Replace("_", " ").ToUpper())
                        {
                            JsonSource = new CustomJsonSource(jsonText),
                            RootElement = ""
                        };

                        jsonStorage.RegisterDataSource(key, jsonDataSource.SaveToXml());
                    }
                }

                configurator.SetDataSourceStorage(jsonStorage);
                configurator.AllowExecutingCustomSql = true;

                // UserId from header
                configurator.CustomParameters += (sender, e) =>
                {
                    var headerValue = httpContext?.Request?.Headers["UserId"].FirstOrDefault();
                    if (long.TryParse(headerValue, out long userId))
                    {
                        var param = e.Parameters.FirstOrDefault(p => p.Name == "UserId");
                        if (param != null)
                        {
                            param.Value = userId;
                            Console.WriteLine("Backend: Overridden UserId = " + userId);
                        }
                    }
                };

                return configurator;
            });

        }
    }


}
