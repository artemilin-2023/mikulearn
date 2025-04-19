using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackBack.Common
{
    /// <summary>
    /// Shared logger for injecting in static classes
    /// </summary>
    public static class LoggerUtil
    {
        private static ILoggerFactory? loggerFactory;
        public static ILoggerFactory LoggerFactory
        {
            get => loggerFactory ?? throw new InvalidOperationException("The LoggerFactory must be setted.");
            set
            {
                if (loggerFactory is not null)
                    throw new InvalidOperationException("The LoggerFactory is already setted.");
                loggerFactory = value;
            }
        }

        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
        public static ILogger CreateLogger(Type type) => LoggerFactory.CreateLogger(type);

        public static void UseStaticLoggerInjecting(this IApplicationBuilder applicationBuilder)
            => LoggerFactory = applicationBuilder.ApplicationServices.GetRequiredService<ILoggerFactory>();
    }

}
