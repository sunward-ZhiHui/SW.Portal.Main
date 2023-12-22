using System;
using System.Globalization;
using System.Reflection;
using AC.SD.Core.Configuration;
using AC.SD.Core.Services;
using Blazored.Toast;
using Core.Repositories.Query;
using DevExpress.Blazor.DocumentMetadata;
using DevExpress.Blazor.RichEdit.SpellCheck;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace AC.SD.Core
{

    public static class DemoServiceCollectionExtensions {
        public static void AddDemoServices(this IServiceCollection services) {
            //services.AddScoped<WeatherForecastService>();
            //services.AddScoped<RentInfoDataService>();
            //services.AddScoped<ContosoRetailDataService>();
            //services.AddScoped<NwindDataService>();
            //services.AddScoped<IssuesDataService>();
            //services.AddScoped<WorldcitiesDataService>();
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
                //opts.Dictionaries.Add(new ISpellDictionary
                //{
                //    DictionaryPath = "Data.Dictionaries.english.xlg",
                //    GrammarPath = "Data.Dictionaries.english.aff",
                //    Culture = "en-US"
                //});
                //opts.Dictionaries.Add(new Dictionary
                //{
                //    DictionaryPath = "Data.Dictionaries.custom.dic",
                //    AlphabetPath = "Data.Dictionaries.english.txt",
                //    Culture = "en-US"
                //});
            });
            services.AddSingleton<RealtimeService>();
            services.AddSingleton<EmailAutoRefresh>();
            services.AddBlazoredToast();            
            services.AddSingleton<DemoConfiguration>();
            services.AddDevExpressServerSideBlazorReportViewer();
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(options =>
            //        {
            //            options.LoginPath = "/login";
            //        });

            //services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAlertService, AlertService>();          
            //services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddIdentity<IdentityUser, IdentityRole>(
                Options => { 
                    Options.Password.RequireDigit = false;
                    Options.Password.RequireLowercase = false;
                    Options.Password.RequireUppercase = false;
                    Options.Password.RequireNonAlphanumeric  = false;
                    Options.SignIn.RequireConfirmedEmail = false;
                });
        }
    }
}
