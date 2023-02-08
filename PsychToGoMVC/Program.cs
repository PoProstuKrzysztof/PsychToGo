using PsychToGo.Interfaces;
using PsychToGo.Repository;
using PsychToGoMVC.Services;
using PsychToGoMVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder( args );
var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPatientService,PatientService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
    .AddCookie( options =>
    {
        options.Cookie.HttpOnly = true;
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LoginPath= "/Auth/Login";
        
        options.ExpireTimeSpan = TimeSpan.FromMinutes( 15 );

        options.SlidingExpiration = true;
    } );


builder.Services.AddSession( options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes( 10 );
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
} );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler( "/Home/Error" );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Psychiatrist}/{action=Index}/{id?}" );


app.Run();
