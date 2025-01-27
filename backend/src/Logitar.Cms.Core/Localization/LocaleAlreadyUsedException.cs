﻿namespace Logitar.Cms.Core.Localization;

public class LocaleAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified locale is already used.";

  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid LanguageId
  {
    get => (Guid)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
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
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(Locale)] = Locale;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public LocaleAlreadyUsedException(Language language, LanguageId conflictId) : base(BuildMessage(language, conflictId))
  {
    ConflictId = conflictId.ToGuid();
    LanguageId = language.Id.ToGuid();
    Locale = language.Locale.ToString();
    PropertyName = nameof(language.Locale);
  }

  private static string BuildMessage(Language language, LanguageId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(LanguageId), language.Id.ToGuid())
    .AddData(nameof(Locale), language.Locale)
    .AddData(nameof(PropertyName), nameof(language.Locale))
    .Build();
}
