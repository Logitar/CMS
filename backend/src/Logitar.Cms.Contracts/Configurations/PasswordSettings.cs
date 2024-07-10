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

  public PasswordSettings() : this(string.Empty)
  {
  }

  public PasswordSettings(string hashingStrategy)
  {
    HashingStrategy = hashingStrategy;
  }
}
