using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using AC.SD.Core.Code;
using AC.SD.Core.Configuration;
using AC.SD.Core.Model;
using AC.SD.Core.Services;
using Microsoft.AspNetCore.Identity;
using Blazored.Toast;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.Blazor.RichEdit.SpellCheck;

namespace AC.SD.Core
{
    public static class DemoServiceCollectionExtensions
    {
        public static void AddDemoServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddDevExpressBlazor(opts =>
            {
                opts.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
            }).AddSpellCheck(opts =>
            {
                opts.FileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "AC.SD.Core");
                opts.MaxSuggestionCount = 6;
                opts.AddToDictionaryAction = (word, culture) =>
                {
                    //Write the selected word to a dictionary file
                };
            });

            services.AddScoped<DefaultDashboard>();
            services.AddSingleton<RealtimeService>();
            services.AddSingleton<EmailAutoRefresh>();
            services.AddBlazoredToast();
            services.AddSingleton<DemoConfiguration>();
            services.AddDevExpressServerSideBlazorReportViewer();

            services.AddIdentity<IdentityUser, IdentityRole>(Options => {
                Options.Password.RequireDigit = false;
                Options.Password.RequireLowercase = false;
                Options.Password.RequireUppercase = false;
                Options.Password.RequireNonAlphanumeric = false;
                Options.SignIn.RequireConfirmedEmail = false;
            });          

            services.AddScoped<IAlertService, AlertService>();
        }
    }
}
