using Bookly.Application.Chains.LoginChain;
using Bookly.Application.Chains.LoginChain.Handlers;
using Bookly.Application.Services;
using Bookly.Application.Services.Files;
using Bookly.Application.Services.Passwords;
using Bookly.Infrastructure;
using Core.Options;
using Microsoft.EntityFrameworkCore;

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
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFilesService, CloudStorageFilesService>();
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