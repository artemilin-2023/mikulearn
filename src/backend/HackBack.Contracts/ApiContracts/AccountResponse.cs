namespace HackBack.Contracts.ApiContracts;

/// <summary>
/// Публичная информация о пользователе.
/// </summary>
/// <param name="Id">Идентификатор пользователя.</param>
/// <param name="Username">Имя пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Roles">Роли пользователя.</param>
public record UserPublic(Guid Id, string Username, string Email, IEnumerable<string> Roles);
