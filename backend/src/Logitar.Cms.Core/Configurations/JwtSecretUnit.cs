using FluentValidation;
using Logitar.Cms.Core.Configurations.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Cms.Core.Configurations;

public record JwtSecretUnit
{
  public const int MinimumLength = 256 / 8;
  public const int MaximumLength = 512 / 8;

  public string Value { get; }

  public JwtSecretUnit(string value)
  {
    Value = value.Trim();
    new JwtSecretValidator().ValidateAndThrow(Value);
  }

  public static JwtSecretUnit CreateOrGenerate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? Generate() : new(value);
  }

  public static JwtSecretUnit Generate() => new(RandomStringGenerator.GetString());
  public static JwtSecretUnit Generate(int length) => new(RandomStringGenerator.GetString(length));
}
