using BinusZoom.Data;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<BinusZoomContext>(options =>
            options.UseMySQL(builder.Configuration.GetConnectionString("BinusZoomContext") ??
                             throw new InvalidOperationException("Connection string 'BinusZoomContext' not found.")));
        
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
            "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}