using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PsychToGo.API.Data;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models.Identity;
using PsychToGo.API.Repository;
using System.Text;

namespace PsychToGo.API.Extensions;

public static class Extensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        //Services injection
        services.AddScoped<IPsychologistRepository, PsychologistsRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IMedicineRepository, MedicineRepository>();
        services.AddScoped<IMedicineCategoryRepository, MedicineCategoryRepository>();
        services.AddScoped<IPsychiatristRepository, PsychiatristRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper( AppDomain.CurrentDomain.GetAssemblies() );
        services.AddTransient<DataSeed>();

        //Security
        services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders();

        services.AddEndpointsApiExplorer();
        services.AddDistributedMemoryCache();
    }

    public static void ConfigureJWT(this IServiceCollection services, WebApplicationBuilder builder)
    {
        string? key = builder.Configuration.GetValue<string>( "ApiSettings:Secret" );

        services.AddSwaggerGen( options =>
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

        services.AddAuthentication( options =>
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
    }
}