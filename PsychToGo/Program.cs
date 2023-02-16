
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PsychToGo;
using PsychToGo.Data;
using PsychToGo.Interfaces;
using PsychToGo.Models.Identity;
using PsychToGo.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder( args );


//CACHING
builder.Services.AddControllers( options =>
{
    options.CacheProfiles.Add( "Cache60",
        new CacheProfile()
        {
            Duration = 60
        } );
} );

builder.Services.AddScoped<IPsychologistRepository, PsychologistsRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<IMedicineCategoryRepository, MedicineCategoryRepository>();
builder.Services.AddScoped<IPsychiatristRepository, PsychiatristRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddAutoMapper( AppDomain.CurrentDomain.GetAssemblies() );
builder.Services.AddTransient<DataSeed>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.AddSecurityDefinition( "Bearer", new OpenApiSecurityScheme()
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 123456abcdf\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    } );

    options.AddSecurityRequirement( new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }

    } );

} );

var key = builder.Configuration.GetValue<string>( "ApiSettings:Secret" );



builder.Services.AddAuthentication( options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
} )
    .AddJwtBearer( JWTOptions =>
    {
        JWTOptions.RequireHttpsMetadata = false;
        JWTOptions.SaveToken = true;
        JWTOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey( Encoding.ASCII.GetBytes( key ) ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    } );




builder.Services.AddDbContext<AppDbContext>( options =>
{
    options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) );
} );

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData( app );

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<DataSeed>();
        service.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();