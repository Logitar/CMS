using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Contracts.Configurations;

public record PasswordSettingsModel : IPasswordSettings
{
  public int RequiredLength { get; set; }
  public int RequiredUniqueChars { get; set; }
  public bool RequireNonAlphanumeric { get; set; }
  public bool RequireLowercase { get; set; }
  public bool RequireUppercase { get; set; }
  public bool RequireDigit { get; set; }
  public string HashingStrategy { get; set; }

  public PasswordSettingsModel() : this(string.Empty)
  {
  }

  public PasswordSettingsModel(IPasswordSettings password) : this(password.HashingStrategy)
  {
    RequiredLength = password.RequiredLength;
    RequiredUniqueChars = password.RequiredUniqueChars;
    RequireNonAlphanumeric = password.RequireNonAlphanumeric;
    RequireLowercase = password.RequireLowercase;
    RequireUppercase = password.RequireUppercase;
    RequireDigit = password.RequireDigit;
  }

  public PasswordSettingsModel(string hashingStrategy)
  {
    HashingStrategy = hashingStrategy;
  }
}
