namespace HackBack.Infrastructure.ServiceRegistration.Options;

/// <summary>
/// Класс для хранения настроек JWT.
/// </summary>
public class JwtOptions
{
    public required string Secret { get; set; }
    public int AccessTokenExpirationHours { get; set; } = 24;
    public int RefreshTokenExpirationHours { get; set; } = 720;
    public TokenValidationOptions TokenValidation { get; set; } = new();
}

public class TokenValidationOptions
{
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public bool ValidateIssuer { get; set; } = false;
    public bool ValidateAudience { get; set; } = false;
    public TimeSpan ClockSkew { get; set; } = TimeSpan.Zero;
}