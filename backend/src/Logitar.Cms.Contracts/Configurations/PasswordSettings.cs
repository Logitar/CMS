using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Contracts.Configurations;

public record PasswordSettings : IPasswordSettings
{
  public int RequiredLength { get; set; }
  public int RequiredUniqueChars { get; set; }
  public bool RequireNonAlphanumeric { get; set; }
  public bool RequireLowercase { get; set; }
  public bool RequireUppercase { get; set; }
  public bool RequireDigit { get; set; }
  public string HashingStrategy { get; set; }

  public PasswordSettings() : this(new PasswordSettings())
  {
  }

  public PasswordSettings(IPasswordSettings password)
    : this(password.RequiredLength, password.RequiredUniqueChars, password.RequireNonAlphanumeric, password.RequireLowercase, password.RequireUppercase, password.RequireDigit, password.HashingStrategy)
  {
  }

  public PasswordSettings(
    int requiredLength,
    int requiredUniqueChars,
    bool requireNonAlphanumeric,
    bool requireLowercase,
    bool requireUppercase,
    bool requireDigit,
    string hashingStrategy)
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;
    HashingStrategy = hashingStrategy;
  }
}
