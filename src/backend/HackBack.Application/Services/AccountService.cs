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
        var result = await Result.TryAsync(async () =>
        {
            var user = await _userRepository.AsQuery()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            ArgumentNullException.ThrowIfNull(user);
            return Result.Success(user);
        }, ex => Error.Failure($"Ошибка при получении пользователя по ID: {ex.Message}"));

        return result;
    }

    public async Task<Result<UserEntity>> GetCurrentUserAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        return await _authService
            .GetUserIdFromAccessToken(request)
            .ThenAsync(id => GetUserByIdAsync(id, cancellationToken))
            .LogIfFailureAsync("Ошибка при получении текущего пользователя");
    }

    public async Task<Result<UserEntity>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var user = await _userRepository.AsQuery()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            ArgumentNullException.ThrowIfNull(user);
            return Result.Success(user);
        }, ex => Error.Failure($"Ошибка при получении пользователя по email: {ex.Message}"));
        return result;
    }

    public async Task<Result<UserEntity>> RegisterAsync(RegisterRequest request, HttpResponse response, CancellationToken cancellationToken)
    {
        var result = await Result.TryAsync(async () =>
        {
            var existingUser = await _userRepository.AsQuery(tracking:true).Where(u => u.Email == request.Email).FirstOrDefaultAsync(cancellationToken);
            if (existingUser != null)
                return Error.Conflict("Пользователь с таким email уже существует");

            var hash = _passwordManager.HashPassword(request.Password);
            if (string.IsNullOrEmpty(hash))
                return Error.Failure("Ошибка хеширования пароля");

            // тут ваще пиздец, я хуй знает как эти роли доставать, мб это не будет работать
            int roleId;
            switch (request.Role)
            {
                case PublicRole.Student:
                    roleId = (int)Role.Student;
                    break;
                case PublicRole.Teacher:
                    roleId = (int)Role.Teacher;
                    break;
                default:
                    roleId = (int)Role.Student;
                    break;
            }

            var roleEntity = await _roleRepository.AsQuery(tracking: true).FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
            if (roleEntity == null)
                return Error.Failure("Не удалось найти роль для пользователя");

            var newUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Username,
                Email = request.Email,
                Password = hash,
                Roles = new List<RoleEntity> { roleEntity }
            };

            var savedUser = await _userRepository.AddAsync(newUser, cancellationToken);
            await _authService.GenerateAndSetTokensAsync(savedUser, response, cancellationToken);
            return Result.Success(savedUser);
        }, ex => Error.Failure($"Ошибка регистрации пользователя: {ex.Message}"));

        return result;
    }

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
    {
        return await _authService.RefreshAccessTokenAsync(request, response, cancellationToken);
    }
}
