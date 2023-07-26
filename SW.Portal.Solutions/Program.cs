using AC.SD.Core.Configuration; 
using SW.Portal.Solutions.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using AC.SD.Core.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blazored.SessionStorage;


public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {

        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(x => Configure(x, args));
    }

    private static void Configure(IWebHostBuilder webHostBuilder, string[] args)
    {
        webHostBuilder
            .UseConfiguration(
                new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("ConnectionStrings.json", false, false)
                    .Build()
            )
            .ConfigureServices(ConfigureServices)            
            .UseStaticWebAssets();
        

        static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {

            services.AddOptions();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddBlazoredSessionStorage();
            services.AddSingleton<DemoConfiguration>();
            services.AddScoped<DemoThemeService>();
            services.AddScoped<IDemoStaticResourceService, DemoStaticResourceService>();
            services.AddDevExpressServerSideBlazorReportViewer();
        }
    }
}
 
