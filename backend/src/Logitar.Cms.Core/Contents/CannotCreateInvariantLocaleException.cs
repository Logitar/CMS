using Logitar.Cms.Contracts.Errors;

namespace Logitar.Cms.Core.Contents;

public class CannotCreateInvariantLocaleException : BadRequestException
{
  private const string ErrorMessage = "A locale cannot be created for an invariant content.";

  public Guid ContentTypeId
  {
    get => (Guid)Data[nameof(ContentTypeId)]!;
    private set => Data[nameof(ContentTypeId)] = value;
  }
  public Guid ContentId
  {
    get => (Guid)Data[nameof(ContentId)]!;
    private set => Data[nameof(ContentId)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public CannotCreateInvariantLocaleException(Content content) : base(BuildMessage(content))
  {
    ContentTypeId = content.ContentTypeId.ToGuid();
    ContentId = content.Id.ToGuid();
  }

  private static string BuildMessage(Content content) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentTypeId), content.ContentTypeId.ToGuid())
    .AddData(nameof(ContentId), content.Id.ToGuid())
    .Build();
}
