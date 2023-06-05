using Microsoft.EntityFrameworkCore;
using SaginPortal.Context;
using SaginPortal.Models;
using SaginPortal.Packages;

namespace SaginPortal;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllersWithViews();

        var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    
        builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString!));
        builder.Services.AddScoped<ExamExistsValidatorAttribute>();

        builder.Services.AddSession(options => {
            options.Cookie.Name = ".MateckiWorker";
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromDays(30);
        });
        
        builder.Services.Configure<AppUrls>(builder.Configuration.GetSection("AppUrls"));

        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(policy => {
                policy.WithOrigins("http://localhost:5289");
            });
        });
                
        var app = builder.Build();
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseExceptionHandler("/Error/500");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        // Session
        app.UseSession();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors();

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