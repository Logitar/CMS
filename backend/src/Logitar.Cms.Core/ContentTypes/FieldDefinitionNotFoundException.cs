using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core.ContentTypes;

public class FieldDefinitionNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified field definition could not be found.";

  public string ContentTypeId
  {
    get => (string)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }
  public Guid FieldId
  {
    get => (Guid)Data[nameof(FieldId)]!;
    private set => Data[nameof(FieldId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PropertyName, $"ContentTypeId={ContentTypeId}, FieldId={FieldId}");

  public FieldDefinitionNotFoundException(ContentTypeAggregate contentType, Guid fieldId, string? propertyName = null)
    : base(BuildMessage(contentType, fieldId, propertyName))
  {
    ContentTypeId = contentType.Id.Value;
    FieldId = fieldId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentTypeAggregate contentType, Guid fieldId, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentTypeId), contentType.Id)
    .AddData(nameof(FieldId), fieldId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
