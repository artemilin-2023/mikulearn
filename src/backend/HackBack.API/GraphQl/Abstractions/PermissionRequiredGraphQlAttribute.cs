using HackBack.API.Abstractions;
using HackBack.Domain.Enums;
using HotChocolate.Authorization;

namespace HackBack.API.GraphQL.Abstractions
{
    public class PermissionRequiredGraphQlAttribute : 
        AuthorizeAttribute,
        IPermissionRequiredAttribute
    {
        public Permission[] Permissions { get; }

        public PermissionRequiredGraphQlAttribute(params Permission[] permissions)
        {
            Permissions = permissions;
            Policy = string.Join(" ", permissions.Order().Select(p => p.ToString()));
        }
    }
}
