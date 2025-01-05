using Logitar.Cms.Core.Localization;

namespace Logitar.Cms.Core.Contents;

public class ContentUniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified content unique name is already used.";

  public Guid ContentTypeId
  {
    get => (Guid)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }
  public Guid? LanguageId
  {
    get => (Guid?)Data[nameof(LanguageId)];
    private set => Data[nameof(LanguageId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      error.Data[nameof(ContentTypeId)] = ContentTypeId;
      error.Data[nameof(LanguageId)] = LanguageId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentUniqueNameAlreadyUsedException(Content content, LanguageId? languageId, ContentLocale locale, ContentId conflictId)
    : base(BuildMessage(content, languageId, locale, conflictId))
  {
    ContentTypeId = content.ContentTypeId.ToGuid();
    LanguageId = languageId?.ToGuid();
    ConflictId = conflictId.ToGuid();
    EntityId = content.Id.ToGuid();
    UniqueName = locale.UniqueName.Value;
    PropertyName = nameof(locale.UniqueName);
  }

  private static string BuildMessage(Content content, LanguageId? languageId, ContentLocale locale, ContentId conflictId)
  {
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(ContentTypeId), content.ContentTypeId.ToGuid())
      .AddData(nameof(LanguageId), languageId?.ToGuid(), "<null>")
      .AddData(nameof(ConflictId), conflictId.ToGuid())
      .AddData(nameof(EntityId), content.Id.ToGuid())
      .AddData(nameof(UniqueName), locale.UniqueName)
      .AddData(nameof(PropertyName), nameof(locale.UniqueName))
      .Build();
  }
}
