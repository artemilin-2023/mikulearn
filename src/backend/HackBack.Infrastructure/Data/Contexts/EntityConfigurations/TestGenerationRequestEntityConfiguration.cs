using HackBack.Domain.Entities;
using HackBack.Infrastructure.Data.Configurations.Configurator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

internal class TestGenerationRequestEntityConfiguration : IEntityTypeConfiguration<TestGenerationRequestEntity>
{
    public void Configure(EntityTypeBuilder<TestGenerationRequestEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsGuid();

        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .IsGuid();

        builder.HasIndex(e => e.CreatedBy);
    }
}
