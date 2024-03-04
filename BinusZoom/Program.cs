using BinusZoom.Controllers;
using BinusZoom.Data;
using BinusZoom.Service;
using BinusZoom.Service.EmailService;
using BinusZoom.Service.ZoomService;
using BinusZoom.Service.ZoomService.DTO;
using BinusZoom.Template.MailTemplate;
using Microsoft.EntityFrameworkCore;

namespace BinusZoom;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Initiate DB Service
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
        builder.Services.AddSingleton<CSMailRenderer>();
        builder.Services.AddSingleton(_ => {
            MailSettings mailSettings = new();
            builder.Configuration.GetSection("MailSettings").Bind(mailSettings);
            return new MailSender(mailSettings);
        });
        
        builder.Services.AddSingleton<ZoomAccountList>(_ =>
        {
            ZoomAccountList zoomAccountList = new ZoomAccountList();
            builder.Configuration.GetSection("ZoomConfig").Bind(zoomAccountList);
            return zoomAccountList;
        });

        builder.Services.AddScoped<ZoomMeetingService>();
        
        // Build the App
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        
        app.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}").RequireRateLimiting("LimitRequest");
        
        app.MapControllers().RequireRateLimiting("LimitRequest");

        app.Run();

        app.Services.GetRequiredService<BinusZoomContext>().Database.Migrate();
    }
}