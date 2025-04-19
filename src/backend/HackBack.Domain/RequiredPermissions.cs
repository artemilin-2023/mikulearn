using HackBack.Domain.Enums;

namespace HackBack.Domain
{
    public record RequiredPermissions
    {
        public static readonly Permission[] CreateTest =
        [
            Permission.Create
        ];
    }
}
