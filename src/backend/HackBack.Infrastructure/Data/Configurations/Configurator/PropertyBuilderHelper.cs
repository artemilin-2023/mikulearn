using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Configurations.Configurator
{
    public static class PropertyBuilderHelper
    {
        public static PropertyBuilder<Guid> IsGuid(this PropertyBuilder<Guid> propertyBuilder)
            => propertyBuilder.HasColumnType("uuid");
    }
}
