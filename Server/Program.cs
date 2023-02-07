using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;

var seed = args.Contains( "/seed" );
if(seed )
{
    args = args.Except( new[] {"/seed"}).ToArray();
}


var builder = WebApplication.CreateBuilder( args );
var assembly = typeof( Program ).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString( "DefaultConnection" );


if (seed)
{
    SeedData.EnsureSeedData( connectionString );
}


builder.Services.AddDbContext<AspNetIdentityDbContext>( options =>
options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore( options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer( connectionString, opt => opt.MigrationsAssembly( assembly ) );
    }
    )
    .AddOperationalStore( options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer( connectionString, opt => opt.MigrationsAssembly( assembly ) );
    } )
    .AddDeveloperSigningCredential();


var app = builder.Build();

app.UseIdentityServer();

app.Run();

