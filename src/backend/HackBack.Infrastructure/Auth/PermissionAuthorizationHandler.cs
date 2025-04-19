using HackBack.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace HackBack.Infrastructure.Auth;

public class PermissionAuthorizationHandler(IServiceProvider services) :
    AuthorizationHandler<PermissionRequirement>, 
    IAuthorizationHandler
{
    private readonly IServiceProvider services = services;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == CustomClaimTypes.UserId);

        if (userId is null || !Guid.TryParse(userId.Value, out var userIdGuid))
            return;

        using var scope = services.CreateAsyncScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var hasPermission = await permissionService.HasPermissionAsync(
            userIdGuid, requirement.Permissions, CancellationToken.None);

        if (hasPermission.IsFailure)
            return;

        context.Succeed(requirement);
    }
}

