using Logitar.Cms.Contracts.Errors;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Localization;

public class LocaleAlreadyUsedException : ConflictException
{
  public const string ErrorMessage = "The specified locale is already used.";

  public string Locale
  {
    get => (string)Data[nameof(Locale)]!;
    private set => Data[nameof(Locale)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public LocaleAlreadyUsedException(LocaleUnit locale, string? propertyName = null) : base(BuildMessage(locale, propertyName))
  {
    Locale = locale.Code;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LocaleUnit locale, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Locale), locale.Code)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
