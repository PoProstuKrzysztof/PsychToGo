using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGoMVC.Services;
using PsychToGoMVC.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );
string connectionString = builder.Configuration.GetConnectionString( "AppDbContextConnection" ) ?? throw new InvalidOperationException( "Connection string 'AppDbContextConnection' not found." );

builder.Services.AddDbContext<AppDbContext>( options => options.UseSqlServer( connectionString ) );

//Identity
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
    .AddCookie( options =>
    {
        options.Cookie.HttpOnly = true;
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LoginPath = "/Auth/Login";

        options.ExpireTimeSpan = TimeSpan.FromMinutes( 15 );

        options.SlidingExpiration = true;
    } );

builder.Services.AddSession( options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes( 10 );
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
} );

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler( "/Home/Error" );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseExceptionHandlerMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}" );

app.Run();