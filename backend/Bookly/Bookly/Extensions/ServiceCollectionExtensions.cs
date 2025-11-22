using System.Reflection;
using System.Text;
using Bookly.Application.Chains.LoginChain;
using Bookly.Application.Chains.LoginChain.Handlers;
using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Handlers.Ratings;
using Bookly.Application.Hangfire;
using Bookly.Application.Services;
using Bookly.Application.Services.ApiScrapers;
using Bookly.Application.Services.Files;
using Bookly.Application.Services.Passwords;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Options;
using MediatR;
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

    public static IServiceCollection AddSwaggerGenWithAuthentication(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            var jwtScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Description = "Введите JWT токен в формате: Bearer {токен}",
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);

            setup.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                { jwtScheme, Array.Empty<string>() }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

    public static IServiceCollection AddGenericMediatRHandlers(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<GetRatingQuery<Book>, Unit>, GetRatingHandler<Book>>();
        services.AddTransient<IRequestHandler<GetRatingQuery<BookCollection>, Unit>, GetRatingHandler<BookCollection>>();
        services.AddTransient<IRequestHandler<AddOrUpdateRatingCommand<Book>, Result>, AddOrUpdateRatingHandler<Book>>();
        services.AddTransient<IRequestHandler<AddOrUpdateRatingCommand<BookCollection>, Result>, AddOrUpdateRatingHandler<BookCollection>>();
        services.AddTransient<IRequestHandler<CalculateAverageRatingQuery<Book>, double>, CalculateAverageRatingHandler<Book>>();
        services.AddTransient<IRequestHandler<CalculateAverageRatingQuery<BookCollection>, double>,
            CalculateAverageRatingHandler<BookCollection>>();
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