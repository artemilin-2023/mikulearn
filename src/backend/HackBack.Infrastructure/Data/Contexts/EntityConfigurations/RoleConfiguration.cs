using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

public class RoleConfiguration :
    IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(r => r.Id);
        var roles = Enum
            .GetValues<Role>()
            .Select(r => new RoleEntity()
            {
                Id = (int)r,
                Name = r.ToString()
            })
            .ToList();

        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles);

        builder.HasData(roles);
    }
}
