using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HackBack.API.Abstractions
{
    public class PermissionRequiredAttribute :
        AuthorizeAttribute,
        IPermissionRequiredAttribute
    {
        public Permission[] Permissions { get; }

        public PermissionRequiredAttribute(params Permission[] permissions)
        {
            Permissions = permissions;
            Policy = string.Join(" ", permissions.Order().Select(p => p.ToString()));
        }
    }
}
