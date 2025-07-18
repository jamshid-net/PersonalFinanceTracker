using System.Reflection;
using FiTrack.Application.Constants;
using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.Category;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Interfaces.Transaction;
using FiTrack.Application.Services.Auth;
using FiTrack.Application.Services.Category;
using FiTrack.Application.Services.Transaction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using TelegramSink;

namespace FiTrack.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        #region AUTH
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICustomIdentityService, CustomIdentityService>();
        builder.Services.AddScoped<IRoleAndPermissionService, RoleAndPermissionService>();
        builder.Services.AddScoped<IAuthUserService, AuthUserService>();
        #endregion

        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IFinanceTransactionService, FinanceTransactionService>();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var logPath = environment == Environments.Development ? "logs" : $"/var/www/BackendLogs/";
        
        var telegramBotToken = builder.Configuration["TelegramConfigure:Token"];
        var telegramChatId = builder.Configuration["TelegramConfigure:ChatId"];
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File(@$"{logPath}/log.log", LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
            .WriteTo.Console(LogEventLevel.Information)
            .CreateLogger();


        TelegramLog.Logger = new LoggerConfiguration().WriteTo
                                                      .TeleSink(telegramBotToken, telegramChatId, null, LogEventLevel.Error)
                                                      .CreateLogger();

    }
}
