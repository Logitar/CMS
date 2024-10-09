using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core.Contents;

public class ContentIdAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified content ID is already used.";

  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public ContentIdAlreadyUsedException(ContentId contentId, string propertyName) : base(BuildMessage(contentId, propertyName))
  {
    Id = contentId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(ContentId contentId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), contentId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
