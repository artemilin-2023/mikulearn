using System.Data.Common;
using HackBack.Application.Abstractions.Auth;
using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.RabbitMQ;
using HackBack.Common;
using HackBack.Contracts.RabbitMQContracts;
using HackBack.Infrastructure.Auth;
using HackBack.Infrastructure.Data;
using HackBack.Infrastructure.Data.Configurations.RegistrationExtension;
using HackBack.Infrastructure.Data.Contexts;
using HackBack.Infrastructure.Data.Repositories;
using HackBack.Infrastructure.HostedServices;
using HackBack.Infrastructure.RabbitMQ.Abstractions;
using HackBack.Infrastructure.RabbitMQ.Consumers.Abstractions;
using HackBack.Infrastructure.RabbitMQ.Consumers.Handlers;
using HackBack.Infrastructure.RabbitMQ.Producers;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using RabbitMQ.Client;

namespace HackBack.Infrastructure.ServiceRegistration;

public static class ServiceRegistrator
{
    public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
        (await services
            .LoadOptions(configuration)
            .AddDbContext(configuration)
            .AddRedisCache(configuration)
            .AddAuth()
            .AddRepositories()
            .AddLlm(configuration) 
            .AddRabbitMqAsync(configuration))
            .AddMinIo(configuration)
            .AddHostedServices();

        return services;
    }
    
    private static IServiceCollection LoadOptions(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.GetAndSaveConfiguration<PasswordManagerOptions>();
        configuration.GetAndSaveConfiguration<Options.AuthorizationOptions>(services);
        configuration.GetAndSaveConfiguration<JwtOptions>(services);
        configuration.GetAndSaveConfiguration<LlmServiceOptions>(services);

        return services;
    }

    private static IServiceCollection AddLlm(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LlmServiceMessageHandler).Assembly));

        return service;

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

    private static async Task<IServiceCollection> AddRabbitMqAsync(this IServiceCollection services, IConfiguration configuration)
    {
        (await services.AddRabbitMqConnectionAsync(configuration))
            .AddRabbitMqProducers(configuration)
            .AddRabbitMqHandlers();

        return services;
    }

    private static async Task<IServiceCollection> AddRabbitMqConnectionAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Rabbit");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var connectoinFactory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        var connection = await connectoinFactory.CreateConnectionAsync();
        
        (connection.IsOpen is false).ThenThrow(new InvalidOperationException($"Couldn't connect to rabbitmq via the connection string {connectionString}"));

        services.AddSingleton(connection);
        services.AddTransient<RabbitClientBuilder>((sp) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var connection = sp.GetRequiredService<IConnection>();

            var builder = new RabbitClientBuilder(connection, loggerFactory.CreateLogger<RabbitClientBuilder>());

            builder.WithLoggerFactory(loggerFactory);

            return builder;
        });

        return services;
    }

    private static IServiceCollection AddRabbitMqProducers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProducer<LlmTestGenerationRequest>, LlmProducer>();
        services.AddScoped<IProducer<GenerateRecommendationRequest>, LlmProducer>();

        return services;
    }

    private static IServiceCollection AddRabbitMqHandlers(this IServiceCollection services)
    {
        services.AddSingleton(typeof(HandlerWrapper<>));
        services.AddTransient<LlmServiceMessageHandler>();

        return services;
    }

    private static IServiceCollection AddMinIo(this IServiceCollection services, IConfiguration configuration)
    {

        DbConnectionStringBuilder builder = new();
        builder.ConnectionString = configuration.GetConnectionString("MinIO")
            ?? throw new InvalidOperationException("MinIo options are not configured");

        services.Configure<MinIoOptions>(options =>
        {
            options.BucketName = (string)builder["bucketname"];
        });

        IMinioClient minioClient = new MinioClient()
            .WithEndpoint((string)builder["host"])
            .WithCredentials((string)builder["username"], (string)builder["password"])
            .WithSSL(false)
            .Build();

        return services.AddSingleton(minioClient)
            .AddScoped<IFileStorage, S3FileStorage>();
    }

    private static TConfiguration GetAndSaveConfiguration<TConfiguration>(this IConfiguration configuration, IServiceCollection? services = null)
        where TConfiguration : class
    {
        var configSection = configuration.GetSection(typeof(TConfiguration).Name);
        (configSection is null).ThenThrow(new InvalidOperationException($"Configuration for {typeof(TConfiguration).Name} is not set"));
        
        var config = configSection!.Get<TConfiguration>();
        (config is null).ThenThrow(new InvalidOperationException($"Configuration for {typeof(TConfiguration).Name} is not set"));

        if (services is null)
            return config!;
        services.Configure<TConfiguration>(configSection!);

        return config!;
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<ResultQueueListener>();
        
        return services;
    }
}