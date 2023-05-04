namespace Logitar.Cms.Core.Configurations;

public record ReadOnlyUsernameSettings : IUsernameSettings
{
  public string? AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
