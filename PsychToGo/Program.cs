using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PsychToGo.API;
using PsychToGo.API.Data;
using PsychToGo.API.Extensions;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models.Identity;
using PsychToGo.API.Repository;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
builder.Services.ConfigureJWT(builder);

builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

WebApplication app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

static void SeedData(IHost app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using IServiceScope scope = scopedFactory.CreateScope();
    DataSeed? service = scope.ServiceProvider.GetService<DataSeed>();
    service.SeedDataContext();
}

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