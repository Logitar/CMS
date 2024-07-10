using Logitar.Cms.Contracts.Errors;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Languages;

public class LocaleAlreadyUsedException : ConflictException
{
  public const string ErrorMessage = "The specified locale is already used.";

  public string LocaleCode
  {
    get => (string)Data[nameof(LocaleCode)]!;
    private set => Data[nameof(LocaleCode)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PropertyName, LocaleCode);

  public LocaleAlreadyUsedException(LocaleUnit locale, string? propertyName = null) : base(BuildMessage(locale, propertyName))
  {
    LocaleCode = locale.Code;
    PropertyName = propertyName;
  }

  private static string BuildMessage(LocaleUnit locale, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(LocaleCode), locale.Code)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
