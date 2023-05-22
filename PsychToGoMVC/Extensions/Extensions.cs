using Microsoft.CodeAnalysis.CSharp.Syntax;
using PsychToGo.Client.Services.Interfaces;
using PsychToGo.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PsychToGo.Client.Extensions;

public static class Extensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        //Services injection
        services.AddControllersWithViews();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IPsychiatristService, PsychiatristService>();
        services.AddScoped<IPsychologistService, PsychologistService>();
        services.AddScoped<IMedicineService, MedicineService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddDistributedMemoryCache();
    }

    public static void ConfigureSecurity(this IServiceCollection service)
    {
        service.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
    .AddCookie( options =>
    {
        options.Cookie.HttpOnly = true;
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LoginPath = "/Auth/Login";
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.ExpireTimeSpan = TimeSpan.FromMinutes( 45 );

        options.SlidingExpiration = true;
    } );

        service.AddSession( options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes( 20 );
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        } );
    }
}