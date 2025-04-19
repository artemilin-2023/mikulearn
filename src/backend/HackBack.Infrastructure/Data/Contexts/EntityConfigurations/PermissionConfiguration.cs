using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

public class PermissionConfiguration :
    IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasMany(p => p.Roles)
            .WithMany(r => r.Permissions)
            .UsingEntity<RolePermissionEntity>(
                left => left.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
                right => right.HasOne<PermissionEntity>().WithMany().HasForeignKey(p => p.PermissionId)
            );

        var permissions = Enum
            .GetValues<Permission>()
            .Select(p => new PermissionEntity
            {
                Id = (int)p,
                Name = p.ToString()
            });

        builder.HasData(permissions);
    }
}
