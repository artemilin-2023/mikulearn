using HackBack.Application.Abstractions.Auth;
using HackBack.Application.Abstractions.Data;
using HackBack.Infrastructure.Auth;
using HackBack.Infrastructure.Data.Configurations.RegistrationExtension;
using HackBack.Infrastructure.Data.Contexts;
using HackBack.Infrastructure.Data.Repositories;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HackBack.Infrastructure.ServiceRegistration;

public static class ServiceRegistrator
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .AddRedisCache(configuration)
            .LoadOptions(configuration)
            .AddRepositories()
            .AddAuth();

        return services;
    }

    private static IServiceCollection LoadOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PasswordManagerOptions>(
            configuration.GetSection(nameof(PasswordManagerOptions)));

        services.Configure<Options.AuthorizationOptions>(
            configuration.GetSection(nameof(Options.AuthorizationOptions)));

        services.Configure<JwtOptions>(
            configuration.GetSection(nameof(JwtOptions)));

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDataAccess<DataContext, DataContextConfigurator>();

        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis options are not configured");

        services.AddStackExchangeRedisCache(options =>
            options.Configuration = redisConnectionString);

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>()
            .AddScoped<IPasswordManager, PasswordManager>()
            .AddScoped<ITokenStorage, TokenStorage>()
            .AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        return services;
    }
}