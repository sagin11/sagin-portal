using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;

namespace SaginPortal;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllersWithViews();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySQL("server=130.162.35.208;user=sagin;password=Sagin#2115_I_Love_Sagin;database=sagin;port=42521");
        });

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
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}