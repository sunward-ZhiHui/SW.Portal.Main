using SW.Portal.Solutions.ServerSide;
public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();


        //FirebaseApp.Create(new AppOptions()
        //{
        //    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serviceAccountKey.json")),
        //});
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    //public static IHostBuilder CreateHostBuilder(string[] args)
    //{

    //    return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(x => Configure(x, args));
    //}

    //private static void Configure(IWebHostBuilder webHostBuilder, string[] args)
    //{

    //    //webHostBuilder
    //    //    .UseConfiguration(
    //    //        new ConfigurationBuilder()
    //    //            .AddCommandLine(args)
    //    //            .SetBasePath(Directory.GetCurrentDirectory())
    //    //            .AddJsonFile("ConnectionStrings.json", false, false)
    //    //            .Build()
    //    //    )
    //    //    .ConfigureServices(ConfigureServices)
    //    //    .UseStaticWebAssets();



    //    static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    //    {

    //        //Enable CORS
    //        //services.AddCors(c =>
    //        //{
    //        //    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    //        //});

    //        //services.Configure<IISServerOptions>(options =>
    //        //{
    //        //    options.AllowSynchronousIO = true;
    //        //});


    //        //services.AddRazorComponents(); // Ensure this is called if required
    //        //services.AddDevExpressBlazor(); // Register DevExpress Blazor services
    //        //services.AddOptions();
    //        //services.AddControllers();
    //        //services.AddHttpContextAccessor();
    //        //services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
    //        //services.AddBlazoredSessionStorage();
    //        //services.AddSingleton<DemoConfiguration>();
    //        //services.AddScoped<DemoThemeService>();
    //        //services.AddScoped<IDemoStaticResourceService, DemoStaticResourceService>();
    //        //services.AddDevExpressServerSideBlazorReportViewer();
    //        //services.AddScoped<ClipboardService>();
    //        //services.AddScoped<FirebaseMessagingService>();


    //        ////services.AddControllersWithViews();
    //        //services.AddHttpClient();

    //        //// Inject IConfiguration to access configuration settings
    //        //var configuration = context.Configuration;

    //        //// Configure strongly typed settings objects
    //        //var appSettingsSection = configuration.GetSection("FcmNotification");
    //        //services.Configure<FcmNotificationSetting>(appSettingsSection);
    //        //services.Configure<RequestBodyOptions>(options =>
    //        //{
    //        //    options.MinRequestBodyDataRate = 1024; // 1 kilobyte per second
    //        //});
    //    }
    //}

}

