using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;

namespace SaginPortal;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllersWithViews();

        var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    
        builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString!));


        builder.Services.AddSession(options => {
            options.Cookie.Name = ".MateckiWorker";
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromDays(30);
        });
        
        var app = builder.Build();
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        // Session
        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "home",
            pattern: "{action=Index}",
            defaults: new { controller = "Home" });
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/");

        app.Run();
    }
}