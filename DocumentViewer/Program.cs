using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc;
using DocumentViewer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    //the change occurs here.
    //builder.cofiguration and not just configuration
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddMvc().AddSessionStateTempDataProvider();
//builder.Services.AddSession();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(100);
    option.Cookie.MaxAge = TimeSpan.FromHours(100);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("_sw_corsPolicy", x =>
    {
        x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();//.AllowCredentials();
    });
});
var app = builder.Build();
app.UseCors("_sw_corsPolicy");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.Use((context, next) => { context.Request.EnableBuffering(); return next(); });


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "node_modules")),
    RequestPath = "/node_modules"
});
app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=home}/{action=Index}/{id?}");

app.Run();
