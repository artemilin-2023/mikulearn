using HackBack.Infrastructure.Data.Configurations.Convertors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HackBack.Infrastructure.Data.Configurations
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder SetDefualtDateTimeKind(this ModelBuilder builder, DateTimeKind kind)
        {
            var converter = new DateTimeKindValueConverter(kind);
            builder.UseValueConverterForType<DateTime>(converter);
            builder.UseValueConverterForType<DateTime?>(converter);
            return builder;
        }

        private static ModelBuilder UseValueConverterForType<T>(this ModelBuilder builder, ValueConverter converter)
            => builder.UseValueConverterForType(typeof(T), converter);

        private static ModelBuilder UseValueConverterForType(this ModelBuilder builder, Type type, ValueConverter converter)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(t => t.PropertyType == type);

                foreach (var property in properties)
                {
                    builder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(converter);
                }
            }

            return builder;
        }
    }
}
