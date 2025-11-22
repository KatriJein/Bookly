using System.Reflection;
using System.Text.Json.Serialization;
using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core;
using Core.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<BooklyOptions>(builder.Configuration);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddBooklyDbContext(builder.Configuration);
builder.Services.AddCors(setup =>
{
    setup.AddDefaultPolicy(config =>
    {
        config
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddJwtAuthenthication(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddProblemDetails(c =>
{
    c.IncludeExceptionDetails = (_, _) => true;
});
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuthentication();
builder.Services.AddHangfire(configuration =>
{
    var booklyOptions = new BooklyOptions();
    builder.Configuration.Bind(booklyOptions);
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(booklyOptions.DbConnectionString));
});
builder.Services.AddHangfireServer();
builder.Services.AddServices();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddGenericMediatRHandlers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();
app.UseProblemDetails();
app.MapHealthChecks("/health");
app.MapControllers();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<BooklyDbContext>();
await dbContext.Database.MigrateAsync();

app.Run();