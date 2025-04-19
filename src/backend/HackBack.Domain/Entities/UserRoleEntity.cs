namespace HackBack.Domain.Entities;

// Это должно быть в инфраструктуре, но так как илюша сделал дженерик репозитории (пасиб),
// мапить домен в инфра энтити невозможно, поэтому теперь все инфра энтити будут в домене (хоть мне это и не нравится).
public class UserRoleEntity
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
}