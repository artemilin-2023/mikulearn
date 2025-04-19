using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using HackBack.Domain.Entities;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.HasMany(t => t.Questions)
               .WithOne(q => q.Test)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
