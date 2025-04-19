namespace HackBack.Domain.Entities;

public class RoleEntity : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public List<PermissionEntity> Permissions { get; set; } = new List<PermissionEntity>();
    public List<UserEntity> Users { get; set; } = new List<UserEntity>();
}