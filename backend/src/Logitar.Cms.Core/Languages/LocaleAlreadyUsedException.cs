using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core.Languages;

public class LocaleAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified locale is already used.";

  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string Locale
  {
    get => (string)Data[nameof(Locale)]!;
    private set => Data[nameof(Locale)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, Locale, PropertyName);
      error.AddData(nameof(ConflictId), ConflictId.ToString());
      return error;
    }
  }

  public LocaleAlreadyUsedException(Language language, LanguageId conflictId) : base(BuildMessage(language, conflictId))
  {
    LanguageId = language.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    Locale = language.Locale.Code;
    PropertyName = nameof(language.Locale);
  }

  private static string BuildMessage(Language language, LanguageId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(LanguageId), language.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(Locale), language.Locale.Code)
    .AddData(nameof(PropertyName), nameof(language.Locale))
    .Build();
}
