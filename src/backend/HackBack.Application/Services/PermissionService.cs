using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Application.Services;

public class PermissionService(IRepository<UserEntity, Guid> userRepository) : IPermissionService
{
    private readonly IRepository<UserEntity, Guid> _userRepository = userRepository;

    public async Task<Result> HasPermissionAsync(Guid userId, Permission[] permissions, CancellationToken cancellationToken)
    {
        try
        {
            var roles = await _userRepository.AsQuery()
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == userId)
            .Select(u => u.Roles)
            .ToListAsync(cancellationToken);

            var userPermissions = roles.SelectMany(r => r).SelectMany(r => r.Permissions).Select(p => (Permission)p.Id).ToHashSet();

            return userPermissions
            .Intersect(permissions).Any()
                ? Result.Success()
                : Error.Forbidden();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure(ex.Message));
        }
    }
}

