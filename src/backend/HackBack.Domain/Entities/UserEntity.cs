namespace HackBack.Domain.Entities;

public class UserEntity : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<RoleEntity> Roles { get; set; } = [];
}