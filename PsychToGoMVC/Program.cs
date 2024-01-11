using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsychToGo.API.Data;
using PsychToGo.Client.Extensions;
using PsychToGo.Client.Middleware;
using PsychToGo.Client.Services;
using PsychToGo.Client.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));

//CACHING
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Cache60",
        new CacheProfile()
        {
            Duration = 60
        });
});

//I'm using service injections through this method, all services are in Extension folder
builder.Services.ConfigureServices();
builder.Services.ConfigureSecurity();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseExceptionHandlerMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();