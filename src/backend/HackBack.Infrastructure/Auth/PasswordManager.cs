using HackBack.Application.Abstractions.Auth;
using HackBack.Infrastructure.ServiceRegistration.Options;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace HackBack.Infrastructure.Auth;

/// <summary>
/// Менеджер паролей для хеширования и проверки паролей.
/// </summary>
/// <remarks>
/// Инициализирует новый экземпляр класса <see cref="PasswordManager"/> с указанными настройками.
/// </remarks>
/// <param name="options">Настройки менеджера паролей.</param>
public class PasswordManager(IOptions<PasswordManagerOptions> options) : 
    IPasswordManager
{
    /// <summary>
    /// Хеширует пароль с использованием BCrypt.
    /// </summary>
    /// <param name="password">Пароль для хеширования.</param>
    /// <returns>Хешированный пароль.</returns>
    /// <exception cref="ArgumentNullException">Возникает, если пароль равен null.</exception>
    /// <exception cref="ArgumentException">Возникает, если пароль пустой.</exception>
    public Result<string> HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return Error.Validation("Пароль не может быть пустым или состоять только из пробелов.");
            
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    /// <summary>
    /// Проверяет, соответствует ли пароль хешу.
    /// </summary>
    /// <param name="password">Пароль для проверки.</param>
    /// <param name="passwordHash">Хеш пароля для сравнения.</param>
    /// <returns>True, если пароль соответствует хешу, иначе false.</returns>
    /// <exception cref="ArgumentNullException">Возникает, если пароль или хеш равны null.</exception>
    /// <exception cref="ArgumentException">Возникает, если пароль или хеш пустые.</exception>
    public Result<bool> VerifyPassword(string password, string passwordHash)
    {
        if (password == null)
            return Error.Validation("Пароль не может быть пустым или состоять только из пробелов.");
        
        if (passwordHash == null)
            return Error.Validation("Хеш пароля не может быть пустым или состоять только из пробелов.");
        
        if (string.IsNullOrWhiteSpace(password))
            return Error.Validation("Пароль не может быть пустым или состоять только из пробелов.");
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Error.Validation("Хеш пароля не может быть пустым или состоять только из пробелов.");

        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}