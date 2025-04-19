namespace HackBack.Domain.Entities;

public class PermissionEntity : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public List<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
}
