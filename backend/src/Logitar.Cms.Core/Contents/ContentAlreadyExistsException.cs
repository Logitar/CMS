namespace Logitar.Cms.Core.Contents;

public class ContentAlreadyExistsException : ConflictException
{
  private const string ErrorMessage = "The specified content already exists.";

  public Guid ContentId
  {
    get => (Guid)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
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
      error.Data[nameof(ContentId)] = ContentId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentAlreadyExistsException(ContentId contentId, string propertyName) : base(BuildMessage(contentId, propertyName))
  {
    ContentId = contentId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentId), contentId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
