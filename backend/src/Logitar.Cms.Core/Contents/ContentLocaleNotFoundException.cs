using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core.Localization;

namespace Logitar.Cms.Core.Contents;

public class ContentLocaleNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified content locale could not be found.";

  public string ContentId
  {
    get => (string)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }
  public string LanguageId
  {
    get => (string)Data[nameof(LanguageId)]!;
    private set => Data[nameof(LanguageId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PropertyName, $"ContentId={ContentId}, LanguageId={LanguageId}");

  public ContentLocaleNotFoundException(ContentAggregate content, LanguageId languageId, string? propertyName)
    : base(BuildMessage(content, languageId, propertyName))
  {
    ContentId = content.Id.Value;
    LanguageId = languageId.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentAggregate content, LanguageId languageId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentId), content.Id)
    .AddData(nameof(LanguageId), languageId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
