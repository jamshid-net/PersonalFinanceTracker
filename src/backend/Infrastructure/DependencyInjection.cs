using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Infrastructure.Data;
using FiTrack.Infrastructure.Data.Interceptors;
using FiTrack.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace FiTrack.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("FiTrack");
        Guard.Against.Null(connectionString, message: "Connection string 'FiTrack' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditLogInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddAsyncSeeding(sp);
        });
        
        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitializer>();
        
        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost"; 
            options.InstanceName = "FiTrack:";
        });

        builder.Services.AddScoped<IAuthCacheService, AuthCacheService>();

        builder.Services.AddSingleton<IRedisServer>(_ =>
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            var endPoint = connectionMultiplexer.GetEndPoints().FirstOrDefault();
            var server = connectionMultiplexer.GetServer(endPoint);
            return new RedisServer(server);
        });


    }
}
