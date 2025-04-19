using HackBack.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HackBack.Infrastructure.Data.Contexts
{
    public static class CustomModelBuilder
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureForecastModel(modelBuilder);
            modelBuilder.SetDefualtDateTimeKind(DateTimeKind.Utc);
        }

        private static void ConfigureForecastModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
