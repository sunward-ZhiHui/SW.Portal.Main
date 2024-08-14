using AC.SD.Core.Helpers;
using AC.SD.Core.ReportProvider;
using AC.SD.Core.Services;
using DevExpress.XtraReports.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Reporting.Core.Services;
using System;
using System.Runtime.InteropServices;
using System.Threading;



namespace AC.ShippingDocument.Reporting
{
    class StartupFilter : IStartupFilter
    {
        static Timer cleaner;
        //static void Clean(string contentRootPath)
        //{
        //    try
        //    {
        //        var reportsDirectory = Path.Join(contentRootPath, DemoReportStorageWebExtension.TempReportsFolderName);
        //        if (!Directory.Exists(reportsDirectory)) return;
        //        var directories = Directory.GetDirectories(reportsDirectory);
        //        foreach (var directory in directories)
        //        {
        //            var files = Directory.GetFiles(directory);
        //            foreach (var file in files)
        //            {
        //                if (DateTime.UtcNow >= File.GetLastAccessTimeUtc(file).AddMinutes(10))
        //                {
        //                    File.Delete(file);
        //                }
        //            }
        //            if (Directory.GetFiles(directory).Length == 0)
        //            {
        //                Directory.Delete(directory);
        //            }
        //        }
        //    }
        //    catch { }
        //}
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return ConfigureApp;

            void ConfigureApp(IApplicationBuilder app)
            {
                app.UseSession();
                //app.UseDevExpressBlazorReporting();
                //app.UseDevExpressServerSideBlazorReportViewer();
                //app.UseReporting(configurator =>
                //{
                //    configurator.DesignAnalyzerOptions.EnableErrorCodeLinks = true;
                //});
                DevExpress.DataAccess.DefaultConnectionStringProvider.AssignConnectionStrings(() => app.ApplicationServices.GetService<ReportingCustomConfigurationProvider>().GetGlobalConnectionStrings());
                var env = app.ApplicationServices.GetService<IWebHostEnvironment>();
               // if (cleaner == null)
                 //   cleaner = new Timer(state => Clean((string)state), env.ContentRootPath, TimeSpan.Zero, TimeSpan.FromSeconds(30));
                next(app);
            }
        }
    }

    public sealed class ReportingHostingStartup
    {
        public static void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((webHostBuilderContext, services) =>
            {
               // var configurationProvider = new ReportingCustomConfigurationProvider(webHostBuilderContext.HostingEnvironment, webHostBuilderContext.Configuration);
                services.AddTransient<IStartupFilter, StartupFilter>();
                services.AddSession();
                services.AddDevExpressServerSideBlazorReportViewer();
                services.AddScoped<IReportProvider, ReportByNameService>();
                //services.AddScoped<IConnectionProviderFactory, ReportDataConnectionProviderFactory>();
                //services.AddScoped<IConnectionProviderService, ReportDataConnectionProviderService>();
                // services.AddDevExpressBlazorReporting();
                 services.AddSingleton<ReportingCustomConfigurationProvider, ReportingCustomConfigurationProvider>();
                services.AddSingleton<DocumentUploadServices, DocumentUploadServices>();
                services.AddSingleton<TreeGenerateRecursive, TreeGenerateRecursive>();
                //DevExpress.Blazor.CompatibilitySettings.ComboBoxCompatibilityMode = true;
                //DevExpress.Blazor.CompatibilitySettings.TagBoxCompatibilityMode = true;

                //services.ConfigureReportingServices((builder) =>
                //{
                //    builder.UseAsyncEngine();
                //    builder.DisableCheckForCustomControllers();
                //    builder.ConfigureReportDesigner(designer =>
                //    {
                //        designer.RegisterDataSourceWizardConfigurationConnectionStringsProvider(configurationProvider.GetReportDesignerWizardConfigurationSection());
                //    });
                //    builder.ConfigureWebDocumentViewer(viewer =>
                //    {
                //        viewer.UseCachedReportSourceBuilder();
                //    });
                //});
                services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
                    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
                });

                services.Configure<IISServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                });

                //services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
                //    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                //    IFileProvider fileProvider = serviceProvider.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider;
                //    return DashboardUtils.CreateDashboardConfigurator(configuration, fileProvider);
                //});

                services.AddTransient<DevExpress.DataAccess.Wizard.Services.ICustomQueryValidator, DevExpress.DataAccess.Wizard.Services.CustomQueryValidator>();
                //services.AddSingleton<IDemoReportSource, DemoReportSource>();
               // services.AddScoped<ReportStorageWebExtension, DemoReportStorageWebExtension>();
               // services.AddScoped(serviceProvider => (IReportProviderAsync)serviceProvider.GetRequiredService<ReportStorageWebExtension>());
                services.AddControllers(options => options.EnableEndpointRouting = false);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    DevExpress.Drawing.Internal.DXDrawingEngine.ForceSkia();
                }
            });
        }
    }
}

