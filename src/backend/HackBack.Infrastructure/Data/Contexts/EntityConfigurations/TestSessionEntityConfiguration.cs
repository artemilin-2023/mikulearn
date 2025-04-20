using HackBack.Domain.Entities;
using HackBack.Infrastructure.Data.Configurations.Configurator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations
{
    internal class TestSessionEntityConfiguration : IEntityTypeConfiguration<TestSessionEntity>
    {
        public void Configure(EntityTypeBuilder<TestSessionEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).IsGuid();

            builder.Property(e => e.StartedAt).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.FinishedAt).IsRequired();
        }
    }
}
