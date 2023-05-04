using System.Globalization;

namespace Logitar.Cms.Contracts.Resources;

public record Locale
{
  public string Code { get; init; } = string.Empty;

  public string DisplayName { get; init; } = string.Empty;

  public string NativeName { get; init; } = string.Empty;

  public static Locale From(CultureInfo culture) => new()
  {
    Code = culture.Name,
    DisplayName = culture.DisplayName,
    NativeName = culture.NativeName,
  };
}
