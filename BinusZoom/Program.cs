using BinusZoom.Controllers;
using BinusZoom.Data;
using BinusZoom.Service;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dbDriver = builder.Configuration.GetValue<String>("DBDriver");
        switch (dbDriver)
        {

            case "MySQL":
                builder.Services.AddDbContext<BinusZoomContext>(options =>
                 options.UseMySQL(builder.Configuration.GetConnectionString("BinusZoomContextMySQL") ??
                           throw new InvalidOperationException("Connection string 'BinusZoomContextMySQL' not found.")));
                break;

            case "SQLServer":
                builder.Services.AddDbContext<BinusZoomContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BinusZoomContextSQLServer") ??
                          throw new InvalidOperationException("Connection string 'BinusZoomContextSQLServer' not found.")));
                break;
        }

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();
       
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // ensure database migrated
            using (var scope = app.Services.GetRequiredService<BinusZoomContext>())
            {
                scope.Database.EnsureCreated();
            }
            
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}").RequireRateLimiting("LimitRequest");

        
        
        app.MapControllers().RequireRateLimiting("LimitRequest");

        app.Run();
    }
}