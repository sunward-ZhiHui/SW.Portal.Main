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
        public static void RegisterDashboard(IServiceCollection services, IConfiguration configuration, IFileProvider fileProvider)
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
                configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
                configurator.SetDataSourceStorage(new DataSourceInMemoryStorage());
                configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

                configurator.ConfigureDataConnection += (s, e) =>
                {
                    //if (e.ConnectionName == "jsonSupport")
                    //{
                    //    Uri fileUri = new Uri(fileProvider.GetFileInfo("Data/Support.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                    //    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    //    jsonParams.JsonSource = new UriJsonSource(fileUri);
                    //    e.ConnectionParameters = jsonParams;
                    //}
                    //if (e.ConnectionName == "jsonCategories")
                    //{
                    //    Uri fileUri = new Uri(fileProvider.GetFileInfo("Data/Categories.json").PhysicalPath, UriKind.RelativeOrAbsolute);
                    //    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    //    jsonParams.JsonSource = new UriJsonSource(fileUri);
                    //    e.ConnectionParameters = jsonParams;
                    //}
                    //if (e.ConnectionName == "jsonCustomers")
                    //{
                    //    JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
                    //    jsonParams.JsonSource = new UriJsonSource(
                    //        new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"));
                    //    e.ConnectionParameters = jsonParams;
                    //}
                };
                return configurator;
            });

        }
    }


}
