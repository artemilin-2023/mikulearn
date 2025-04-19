using HackBack.Domain.Entities;
using HackBack.Infrastructure.Data.Contexts.EntityConfigurations;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HackBack.Infrastructure.Data.Contexts
{
    public class DataContext(
        DbContextOptions<DataContext> options,
        IOptions<AuthorizationOptions> authorizationOptions)
        : DbContext(options)
    {

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<TestEntity> Tests { get; set; }
        public DbSet<QuestionEntity> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CustomModelBuilder.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(authorizationOptions.Value));
        }
    }
}