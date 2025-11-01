using Bookly.Application.Services;
using Bookly.Application.Services.Files;
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
        services.AddHostedService<GenresSeedService>();
        return services;
    }
}