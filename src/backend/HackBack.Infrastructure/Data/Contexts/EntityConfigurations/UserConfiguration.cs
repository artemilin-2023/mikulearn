using HackBack.Domain.Entities;
using HackBack.Infrastructure.Data.Configurations.Configurator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HackBack.Infrastructure.Data.Contexts.EntityConfigurations;

public class UserConfiguration :
    IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsGuid();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRoleEntity>(
                left => left.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
                right => right.HasOne<UserEntity>().WithMany().HasForeignKey(u => u.UserId)
            );
    }
}
