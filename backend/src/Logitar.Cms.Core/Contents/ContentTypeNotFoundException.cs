namespace Logitar.Cms.Core.Contents;

public class ContentTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified content type could not be found.";

  public Guid ContentTypeId
  {
    get => (Guid)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
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
      error.Data.Add(nameof(ContentTypeId), ContentTypeId);
      error.Data.Add(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public ContentTypeNotFoundException(ContentTypeId contentTypeId, string propertyName) : base(BuildMessage(contentTypeId, propertyName))
  {
    ContentTypeId = contentTypeId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentTypeId contentTypeId, string propertyName) => new ErrorMessageBuilder()
    .AddData(nameof(ContentTypeId), contentTypeId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
