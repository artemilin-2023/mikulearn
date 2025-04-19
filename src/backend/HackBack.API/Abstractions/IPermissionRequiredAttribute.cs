using HackBack.Domain.Enums;

namespace HackBack.API.Abstractions
{
    public interface IPermissionRequiredAttribute
    {
        string? Policy { get; }
        Permission[] Permissions { get; }
    }
}
