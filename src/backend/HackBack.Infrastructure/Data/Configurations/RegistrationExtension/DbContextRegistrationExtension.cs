using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HackBack.Infrastructure.Data.Configurations.RegistrationExtension
{
    public static class DbContextRegistrationExtension
    {
        public static IServiceCollection AddPostgresDataAccess<TContext, TContextConfigurator>(this IServiceCollection services)
            where TContext : DbContext
            where TContextConfigurator : class, IDbContextOptionsConfigurator<TContext>
        {
            services.AddDbContextPool<TContext>(Configure<TContext>);
            
            services.AddSingleton<IDbContextOptionsConfigurator<TContext>, TContextConfigurator>()
                .AddScoped<DbContext>(t => t.GetRequiredService<TContext>());

            return services;
        }

        internal static void Configure<TContext>(IServiceProvider sp, DbContextOptionsBuilder optionsBuilder) where TContext: DbContext
        {
            var configuration = sp.GetRequiredService<IDbContextOptionsConfigurator<TContext>>();
            configuration.Configure((DbContextOptionsBuilder<TContext>)optionsBuilder);
        }
    }
}
