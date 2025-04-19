using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

public class RolePermissionConfiguration(AuthorizationOptions authorizationOptions) : IEntityTypeConfiguration<RolePermissionEntity>
{
    private readonly AuthorizationOptions authorizationOptions = authorizationOptions;

    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        var data = authorizationOptions.Permissions
            .SelectMany(rp => rp.Permissions
                .Select(p => new RolePermissionEntity()
                {
                    RoleId = (int)Enum.Parse<Role>(rp.Role),
                    PermissionId = (int)Enum.Parse<Permission>(p)
                })).ToArray();
        builder.HasData(data);
    }
}
