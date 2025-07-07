using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;
using UserManagement.API.DI;
using UserManagement.API.Helpers;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;
using UserManagement.Infrastructure.Data.SeedData;
using UserManagement.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<AppDbContext>(x => x.UseSqlServer(config.GetConnectionString("DefaultConnection")));
services.AddIdentity<AppUser, AppRole>()
 .AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


var jwtConfig = builder.Configuration.GetSection("JWT").Get<JWT>();

services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Request.Headers["X-Client-Id"].FirstOrDefault() ?? httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));


    options.AddPolicy("strict", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));


    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.",
            cancellationToken: cancellationToken);
        return ValueTask.CompletedTask;
    };
});

services.AddAuthorization();
services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
        .AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig!.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                RequireExpirationTime = true,
            };
        });
services.AddSwaggerGen(s =>
{
    s.ResolveConflictingActions(apiDesc => apiDesc.First());
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
    var Scheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme,
        },
    };
    s.AddSecurityDefinition(Scheme.Reference.Id, Scheme);
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            { Scheme, Array.Empty<string>() },
        });
    s.OperationFilter<AddBearerPrefixFilter>();


});
services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});


services.AddApplicationServices();

services.AddLocalization(options => options.ResourcesPath = "Resources");
services.Configure<RequestLocalizationOptions>(option =>
{
    var supprtedCultures = new[]
    {
        new CultureInfo(Language.en.ToString()),
        new CultureInfo(Language.hi.ToString()),

    };

    option.DefaultRequestCulture = new RequestCulture(Language.en.ToString());
    option.SupportedCultures = supprtedCultures;
    option.SupportedUICultures = supprtedCultures;

    option.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var scopeServices = scope.ServiceProvider;
    var loggerFactory = scopeServices.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = scopeServices.GetRequiredService<AppDbContext>();
        var userManager = scopeServices.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scopeServices.GetRequiredService<RoleManager<AppRole>>();
        await context.Database.MigrateAsync();
        await SeedAppData.SeedAsync(context, loggerFactory, userManager, roleManager);

    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError($"An error occurred during migration: {ex.Message}");
    }
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseRequestLocalization();
app.MapControllers();

app.Run();
