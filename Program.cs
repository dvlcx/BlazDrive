using BlazDrive.Components;
using BlazDrive.Data;
using BlazDrive.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlazDrive;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(ops =>
            {
                ops.LoginPath = "/login";
                ops.Cookie.Name = "auth_token";
                ops.Cookie.MaxAge = TimeSpan.FromMinutes(30);
                ops.AccessDeniedPath = "/error";
            });
        builder.Services.AddControllers();
        builder.Services.AddMemoryCache();
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddDbContextFactory<AppDbContext>(options => 
            options.UseMySql(builder.Configuration.GetConnectionString("BlazDriveConnectionString"),
                new MySqlServerVersion("11.2.3-MariaDB")));
        builder.Services.AddSingleton<AccountMainService>();
        builder.Services.AddTransient<AccountInfoService>();
        builder.Services.AddSingleton<AccountEditService>();
        builder.Services.AddSingleton<BlazDriveStorageService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        
        app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.MapControllers();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.Run();
    }
}
