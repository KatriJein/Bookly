using System.Text;
using Bookly.Application.Chains.LoginChain;
using Bookly.Application.Chains.LoginChain.Handlers;
using Bookly.Application.Hangfire;
using Bookly.Application.Services;
using Bookly.Application.Services.ApiScrapers;
using Bookly.Application.Services.Files;
using Bookly.Application.Services.Passwords;
using Bookly.Infrastructure;
using Core;
using Core.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Bookly.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBooklyDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var booklyOptions = new BooklyOptions();
        configuration.Bind(booklyOptions);
        services.AddDbContext<BooklyDbContext>(o 
            => o.UseNpgsql(booklyOptions.DbConnectionString, b
                => b.MigrationsAssembly(typeof(BooklyDbContext).Assembly)).LogTo(Console.WriteLine, LogLevel.Information));
        return services;
    }
    
    public static IServiceCollection AddJwtAuthenthication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKeyInfo = configuration[Const.JwtSecretKey] ?? "r";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyInfo));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = "Bookly",
                ValidAudience = "Bookly",
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey
            };
        });
        return services;
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFilesService, CloudStorageFilesService>();
        services.AddScoped<IBooksApiScraperService, BooksApiScraperService>();
        services.AddHostedService<CreateHangfireJobsService>();
        services.AddHostedService<PublishersSeedService>();
        services.AddHostedService<GenresSeedService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddLoginChain();
        return services;
    }

    private static IServiceCollection AddLoginChain(this IServiceCollection services)
    {
        services.AddScoped<LoginAsEmailHandler>();
        services.AddScoped<LoginAsUsernameHandler>();
        services.AddScoped<ILoginChain, LoginChain>();
        return services;
    }
}