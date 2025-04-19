namespace HackBack.Infrastructure.ServiceRegistration.Options;

/// <summary>
/// Конфигурационные параметры для менеджера паролей.
/// </summary>
public class PasswordManagerOptions
{
    /// <summary>
    /// Соль для хеширования паролей.
    /// </summary>
    public required string Salt { get; set; }

    /// <summary>
    /// Флаг, указывающий, следует ли использовать улучшенную энтропию.
    /// </summary>
    public bool EnhancedEntropy { get; set; } = true;
}