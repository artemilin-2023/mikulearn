using HackBack.Application.Abstractions.Auth;
using HackBack.Application.Abstractions.Data;
using HackBack.Application.Abstractions.Services;
using HackBack.Contracts.ApiContracts;
using HackBack.Domain.Entities;
using HackBack.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Extensions.FunctionalExtensions.Async;
using ResultSharp.Logging;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Extensions.TransformationExtensions;

namespace HackBack.Application.Services;

public class AccountService(
        IRepository<UserEntity, Guid> userRepository,
        IRepository<RoleEntity, int> roleRepository,
        IAuthService authService,
        IPasswordManager passwordManager) : IAccountService
{
    private readonly IRepository<UserEntity, Guid> _userRepository = userRepository;
    private readonly IRepository<RoleEntity, int> _roleRepository = roleRepository;
    private readonly IAuthService _authService = authService;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<Result<UserEntity>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.AsQuery()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                .ToResultAsync([u => u is not null])
                .MapAsync(u => u!);

        return user;
    }

    public async Task<Result<UserEntity>> GetCurrentUserAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService
            .GetUserIdFromHttpRequest(request)
            .ThenAsync(id => GetUserByIdAsync(id, cancellationToken))
            .LogIfFailureAsync("Ошибка при получении текущего пользователя");

        return user;
    }

    public async Task<Result<UserEntity>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userRepository.AsQuery()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
                .ToResultAsync([u => u is not null])
                .MapAsync(u => u!);

        return user;
    }

    public async Task<Result<UserEntity>> RegisterAsync(RegisterRequest request, HttpResponse response, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository
            .AsQuery(tracking:true)
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser != null)
            return Error.Conflict("Пользователь с таким email уже существует");

        var hash = _passwordManager.HashPassword(request.Password);
        if (hash.IsFailure)
            return Error.Failure("Ошибка хеширования пароля.");

        var roleId = request.Role switch
        {
            PublicRole.Student => (int)Role.Student,
            PublicRole.Teacher => (int)Role.Teacher,
            _ => (int)Role.Student,
        };

        var roleEntity = await _roleRepository
            .AsQuery(tracking: true)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (roleEntity == null)
            return Error.Failure("Не удалось найти роль для пользователя");

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Username,
            Email = request.Email,
            Password = hash,
            Roles = [roleEntity]
        };

        var savedUser = await _userRepository.AddAsync(newUser, cancellationToken);
        await _authService.GenerateAndSetTokensAsync(savedUser, response, cancellationToken);

        return savedUser;
    }

    // крч кому то было лень прочитать доку к либе и нахуя то тут везду Result.TryAsync, внутри которого выбравсывается исключение (реазалт паттерн в ахуе),
    // часть я отрефакторил, но я так понимаю, что похожий код тут много где. Мне влом рефакторить все. Работает и хуй с ним.
    public async Task<Result> LoginAsync(LoginRequest request, HttpResponse response, CancellationToken cancellationToken)
    {

        var result = await Result.TryAsync(
            async () =>
            {
                var user = await _userRepository.AsQuery().Where(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
                ArgumentNullException.ThrowIfNull(user);
                if (!_passwordManager.VerifyPassword(request.Password, user.Password))
                    return Error.Unauthorized("Неверный email или пароль");

                await _authService.GenerateAndSetTokensAsync(user, response, cancellationToken);
                return Result.Success();

            }, ex => Error.Failure($"Ошибка при входе: {ex.Message}"));

        return result;
    }

    public Task<Result> LogoutAsync(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        => _authService.ClearTokensAsync(request, response, cancellationToken);

    public async Task<Result> RefreshToken(HttpRequest request, HttpResponse response, CancellationToken cancellationToken)
        => await _authService.RefreshAccessTokenAsync(request, response, cancellationToken);
}
